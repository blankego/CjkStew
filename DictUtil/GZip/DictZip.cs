using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace DictUtil
{
	public class DzExtraField : GzExtraField
	{
		public static readonly ushort DEFAULTCHUNKLEN = 58315;
		public ushort Version {get; private set;}
		ushort _chLen = DEFAULTCHUNKLEN;
		public ushort ChunkLength {
			get{ return _chLen; }
			set{
				if(value < 1024 || value > DEFAULTCHUNKLEN)
				throw new ArgumentOutOfRangeException(
					"The chunk size must be less than 64KB and yet it shouldn't be" +
					" too small or the compression ratio would degrade dramatically");
				_chLen = value;
			}
		}
		public ushort ChunkCount{get; private set;}

		void UpdateChunkCount(ushort chCnt)
		{
			FileStream.Position = FieldDataBegin + 4;
			FileStream.SetIntLittleEndian(chCnt,2);
		}

		uint[] _indices;
		public uint[] Indices{ get {return _indices; } }

		internal override void ReadFrom ()
		{

			base.ReadFrom();
			_fs.Position = FieldDataBegin;
			Version = (ushort)_fs.GetIntLittleEndian(2);
			ChunkLength = (ushort)_fs.GetIntLittleEndian(2);
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

		internal override void WriteTo ()
		{
			base.WriteTo();
			_fs.Position =  FieldDataBegin;
			_fs.SetIntLittleEndian((uint)( Version = 1),2);
			_fs.SetIntLittleEndian(ChunkLength);
			UpdateChunkCount(ChunkCount = 0);
		}

	}

	public class DictZip : GZipBase<DzExtraField> , IDictDB
	{

		byte[] _buf;
		int _lastStart = -1, _lastEnd = -1;
		string _tempName;
		FileStream _temp;
		Encoding _enc;

		public int ChunkCount { get { return ExtraField.ChunkCount; } }

		public int ChunkLength { get { return ExtraField.ChunkLength; } }

		private DictZip(string path, FileMode mode, Encoding enc = null) :base(path, mode)
		{
			_enc = enc ?? Encoding.UTF8;

			//Because the index table of chunks keeps growing as the compressed data grows
			//they're better handled separately. At the disposing phase these two parts
			//will be concatenated to produce the final target archive.
			if(IsCreating)
			{
				_temp = File.Create(_tempName = Path.GetTempFileName());
				ExtraField = new DzExtraField();
			}
		}

		public byte[] ReadAt (int pos, int cnt)
		{
			EnsureReadMode();
			int end = pos + cnt - 1;
			if (pos < 0 || end > OriginalSize)
				throw new ArgumentOutOfRangeException();
			int startIdx = pos / ExtraField.ChunkLength;
			int offset = pos % ExtraField.ChunkLength;
			int endIdx = end / ExtraField.ChunkLength;

			if(startIdx != _lastStart || endIdx != _lastEnd || _buf == null)
			{			
				_lastStart = startIdx;
				_lastEnd = endIdx;
				_buf = new byte[(endIdx - startIdx + 1) * ExtraField.ChunkLength];
				//read chunks
				for (int i = startIdx; i <= endIdx; ++i)
				{
					FileStream.Position = _dataBegin + ExtraField.Indices[i];
					DeflateStream.Dispose();
					DeflateStream = new DeflateStream(FileStream, CompressionMode.Decompress,true);
					//If I keep using the same deflatestream the data will become corrupted 
					//when crossing chunks, I don't no why :(
						
					Read(_buf, (i - startIdx) * ExtraField.ChunkLength, ExtraField.ChunkLength);
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

		protected override void WriteTo (byte[] buf, int offset, int cnt)
		{
			base.WriteTo(buf, offset, cnt);
		}



		static public DictZip OpenRead(string path, Encoding enc = null)
		{
			return new DictZip(path, FileMode.Open, enc);
		}

		static public DictZip Create(string path, bool overwriteIfExists = false, Encoding enc = null)
		{
			if(!overwriteIfExists && File.Exists(path))
				throw new IOException("File "+path+" already exists!");
			return new DictZip(path, FileMode.Create, enc);
		}
	}
}

