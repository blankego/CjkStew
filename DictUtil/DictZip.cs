using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace DictUtil
{
	public class DzExtraField : GzExtraField
	{
		public readonly ushort Version, ChunkLength, ChunkCount;
		public readonly uint[] Indices;

		unsafe internal DzExtraField (Stream fs): base(fs)
		{
			fs.Position = FieldDataBegin;
			Version = (ushort)(fs.ReadByte() | (fs.ReadByte() << 8));
			ChunkLength = (ushort)(fs.ReadByte() | (fs.ReadByte() << 8));
			ChunkCount = (ushort)(fs.ReadByte() | (fs.ReadByte() << 8));

			Indices = new uint[ChunkCount + 1];
			Indices[0] = 0;

			//read index
			int idxTableSize = ChunkCount * (Version == 1 ? 2 : 4); //seems only version 1 exists

			byte[] buf = new byte[idxTableSize];

			for (int bytesRead = 0, chSize = 1; bytesRead < idxTableSize && chSize > 0;)
				bytesRead += chSize = fs.Read(buf, bytesRead, idxTableSize - bytesRead);

			//convert individual chunk sizes to offsets
			for (uint sum = 0,  i = 0; i < ChunkCount; ++i)
			{
				sum += (uint)(buf[i * 2] | (buf[i * 2 + 1] << 8));
				Indices[i + 1] = sum;
			}

		}

	}

	public class DictZip : GZipBase<DzExtraField> , IDictDB
	{

		byte[] _buf;
		int _lastStart = -1, _lastEnd = -1;
		Encoding _enc;

		public int ChunkCount { get { return ExtraField.ChunkCount; } }

		public int ChunkLength { get { return ExtraField.ChunkLength; } }


		public DictZip (string path, Encoding enc = null) : base (path)
		{
			_enc = enc ?? Encoding.UTF8;
		}

		protected override void ReadExtraField ()
		{
			ExtraField = new DzExtraField(FileStream);
		}

		public byte[] ReadAt (int pos, int cnt)
		{
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
				
					DeflateStream = new DeflateStream(FileStream, CompressionMode.Decompress);
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

	}
}

