using System;
using System.IO;

namespace DictUtil
{
	public class Crc32
	{
		public const uint POLYNOMIAL = 0xedb88320, SEED = 0xffffffff;
		static readonly int ARRLEN = 4 * 1024;
		static readonly uint[] _table = new uint[256];
		static readonly byte[] _buf = new byte[ARRLEN];

		static Crc32 ()
		{                        
			for (uint i = 0; i < _table.Length; ++i)
			{
				uint c = i;
				for (int j = 8; j > 0; --j)
				{
					if ((c & 1) == 1)					
						c = (c >> 1) ^ POLYNOMIAL;
					else 
						c >>= 1;                    
				}
				_table[i] = c;
			}
		}

		static public uint UpdateCrc (byte[] buf, int offset, int count, uint crc = 0 )
		{
			crc ^= SEED;
			for (int i = offset; i < offset + count; ++i)
			{
				crc = _table[(crc ^ buf[i]) & 0xff] ^ (crc >> 8);
			}
			return ~crc;
		}

		static public uint Compute (Stream s)
		{
			Array.Clear(_buf, 0, ARRLEN);
			uint crc = 0;
			for (int bytesRead = 0, chunkSize = 1; chunkSize > 0; bytesRead = 0)
			{
				while (bytesRead < ARRLEN && chunkSize > 0)
					bytesRead += chunkSize = s.Read(_buf, 0, ARRLEN);
				crc = UpdateCrc(_buf, 0, bytesRead, crc);
			}
			return crc;
		}

		static public uint Compute (byte[] arr)
		{
			return UpdateCrc(arr, 0, arr.Length);
		}
	}
}

