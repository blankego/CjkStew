using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace DictUtil
{
	public class DzExtraField : GzExtraField
	{
		protected readonly byte[] _id = {82,65};
		public static readonly ushort DEFAULTCHUNKLEN = 58315;
		public ushort Version {get; private set;}
		ushort _chLen = DEFAULTCHUNKLEN;
		public ushort ChunkSize {
			get{ return _chLen; }

			//may only set once
			internal set{
				if(value < 1024 || value > DEFAULTCHUNKLEN)
				throw new ArgumentOutOfRangeException(
					"The chunk size must be less than 64KB and yet it shouldn't be" +
					" too small or the compression ratio would degrade dramatically");
				_chLen = value;
			}
		}
		public ushort ChunkCount{get; private set;}

		internal void UpdateChunkCount()
		{
			FileStream.Position = FieldDataBegin + 4;
			FileStream.SetIntLittleEndian((uint)(FileStream.Length - IndexTableBegin),2);
			FileStream.Seek(0,SeekOrigin.End);
		}

		uint[] _indices;
		public uint[] Indices{ get {return _indices; } }
		public int IndexTableBegin { get { return FieldDataBegin + 6; } }

		internal override void Read ()
		{

			base.Read();
			_fs.Position = FieldDataBegin;
			Version = (ushort)_fs.GetIntLittleEndian(2);
			ChunkSize = (ushort)_fs.GetIntLittleEndian(2);
			ChunkCount = (ushort)_fs.GetIntLittleEndian(2);

			_indices = new uint[ChunkCount + 1];
			_indices[0] = 0;

			//read index
			int idxTableSize = ChunkCount * (Version == 1 ? 2 : 4); //seems only version 1 exists

			byte[] buf = new byte[idxTableSize];

			for (int bytesRead = 0, chSize = 1; bytesRead < idxTableSize && chSize > 0;)
				bytesRead += chSize = _fs.Read(buf, bytesRead, idxTableSize - bytesRead);

			//convert individual chunk sizes to offsets
			for (uint sum = 0,  i = 0; i < ChunkCount; ++i)
			{
				sum += (uint)(buf[i * 2] | (buf[i * 2 + 1] << 8));
				Indices[i + 1] = sum;
			}

		}

		internal void WritePreliminary ()
		{
			_fs.Seek(12,SeekOrigin.Begin);
			_fs.WriteByte((byte)'R');
			_fs.WriteByte((byte)'A');
			//LEN skipped
			_fs.Position =  FieldDataBegin;
			_fs.SetShortLittleEndian((ushort)( Version = 1));
			_fs.SetShortLittleEndian(ChunkSize);//
			_fs.SetShortLittleEndian(0); //chunk count will be set after the compression is done
		}

		internal void Finish()
		{
			Length = (int)(_fs.Length - FieldDataBegin);
			_fs.Position = 10;
			_fs.SetShortLittleEndian((ushort)XLength);
			_fs.Position += 2;
			_fs.SetShortLittleEndian((ushort)Length);
			_fs.Position = FieldDataBegin + 4;
			//chunk count
			_fs.SetShortLittleEndian((ushort)((Length - 6)/2));
		}
	}

	//ugly ! 

	public class DictZip : GZipBase<DzExtraField> , IDictDB
	{
		const int BUFSIZE = 1024 * 64;
		byte[] _buf;
		int _lastStart = -1, _lastEnd = -1, _chOffset = 0;
		long _lastChunkEnd;
		string _tempName;
		FileStream _temp;
		Encoding _enc;


		int SpaceToFill { get { return ExtraField.ChunkSize - _chOffset; } }

		private DictZip(string path, int chunkSize, FileMode mode, Encoding enc = null) :base(path, mode)
		{
			_enc = enc ?? Encoding.UTF8;
			
			//Because the index table of chunks keeps growing as the compressed data grows
			//they're better handled separately. At the disposing phase these two parts
			//will be concatenated to produce the final target archive.
			if(IsCreating)
			{
				_temp = File.Create(_tempName = Path.GetTempFileName());
				_lastChunkEnd = 0;
				ExtraField = new DzExtraField{IsCreating = true, FileStream = FileStream};
				if(chunkSize != 0)ExtraField.ChunkSize = (ushort)chunkSize;

				DeflateStream = new DeflateStream(_temp, CompressionMode.Compress, true);
				WriteHeader();
				ExtraField.WritePreliminary();
			}
		}

		public byte[] ReadAt (int pos, int cnt)
		{
			EnsureReadMode();
			int end = pos + cnt - 1;
			if (pos < 0 || end > OriginalSize)
				throw new ArgumentOutOfRangeException();
			int startIdx = pos / ExtraField.ChunkSize;
			int offset = pos % ExtraField.ChunkSize;
			int endIdx = end / ExtraField.ChunkSize;

			if(startIdx != _lastStart || endIdx != _lastEnd || _buf == null)
			{			
				_lastStart = startIdx;
				_lastEnd = endIdx;
				_buf = new byte[(endIdx - startIdx + 1) * ExtraField.ChunkSize];
				//read chunks
				for (int i = startIdx; i <= endIdx; ++i)
				{
					FileStream.Position = _dataBegin + ExtraField.Indices[i];
					DeflateStream.Dispose();
					DeflateStream = new DeflateStream(FileStream, CompressionMode.Decompress,true);
					//If I keep using the same deflatestream the data will become corrupted 
					//when crossing chunks, I don't no why :(
						
					Read(_buf, (i - startIdx) * ExtraField.ChunkSize, ExtraField.ChunkSize);
				}
			}
			byte[] res = new byte[cnt];
			Buffer.BlockCopy(_buf, offset, res, 0, cnt);
			return res;
		}

		public byte[] GetBytes(long pos, int cnt)
		{
			return ReadAt((int)pos, cnt);
		}

		public string GetEntry (long pos, int cnt)
		{
			return _enc.GetString(ReadAt((int)pos, cnt));
		}

		public override byte[] ReadAllBytes ()
		{
			if (OriginalSize > 100 * 1024 * 1024)
				throw new NotSupportedException("The file is to big to read its entire content at once!");
			return ReadAt(0, (int)OriginalSize);
		}
		protected override void EnsureHeaderWritten ()
		{
			EnsureWriteMode();
		}
		void ChunkDone()
		{

			// the deflate stream must be flushed first so that the size of underlying file can 
			//be updated accordingly
			DeflateStream.Dispose();
			//add a idx entry
			FileStream.SetShortLittleEndian((ushort)(_temp.Length - _lastChunkEnd));
			_lastChunkEnd = _temp.Length;
			DeflateStream = new DeflateStream(_temp,CompressionMode.Compress,true);
			_chOffset = 0;
		}

		protected override void WriteTo (byte[] buf, int offset, int cnt)
		{
			while(cnt > 0)
			{
				int size = cnt > SpaceToFill ? SpaceToFill : cnt;
				base.WriteTo(buf, offset, size);

				if(size == SpaceToFill)
					ChunkDone();
				else
					_chOffset += size;

				cnt -= size;
				offset += size;
			}
		}



		static public DictZip OpenRead(string path, Encoding enc = null)
		{
			return new DictZip(path, 0,FileMode.Open, enc);
		}

		static public DictZip Create(string path, bool overwriteIfExists = false, Encoding enc = null, int chunkSize = 0)
		{
			if(!overwriteIfExists && File.Exists(path))
				throw new IOException("File "+path+" already exists!");
			return new DictZip(path, chunkSize, FileMode.Create, enc);

		}
		public override void Dispose ()
		{
			if(!IsCreating)
				base.Dispose();
			else
			{
				if(_chOffset > 0)ChunkDone();
				DeflateStream.Dispose();
				_temp.Dispose();
				ExtraField.Finish();
				FileStream.Seek(0, SeekOrigin.End);
				WriteNameComment();
				using(FileStream temp = File.OpenRead(_tempName))
				{
					temp.CopyTo(FileStream);
				}
				WriteFooter();
				FileStream.Dispose();
				File.Delete(_tempName);
			}
		}
	}
}

