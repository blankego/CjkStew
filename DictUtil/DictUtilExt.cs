using System;
using System.IO;

namespace DictUtil
{
	public static class DictUtilExt
	{
		static public ulong GetUlongBigEndian (this Stream s, int size = 8)
		{
			ulong res = 0;
			for(;size-- > 0; res |= (ulong)s.ReadByte() << (size * 8));
			return res;
		}

		static public uint GetIntBigEndian (this Stream s, int size = 4)
		{
			uint res = 0;
			for(;size-- > 0; res |= (uint)s.ReadByte() << (size * 8));
			return res;
		}

		static public uint GetIntLittleEndian (this Stream s, int size = 4)
		{
			uint res = 0;
			for(int i = 0; i < size; ++i)
				res |= (uint)s.ReadByte() << (i * 8);
			return res;
		}

		static public void SetIntLittleEndian (this Stream s, uint num, int size = 4)
		{
			for (int i = 0; i < 4; ++i)
			{
				s.WriteByte((byte)(num >> i * 8));
			}
		}
		 
		static public ulong GetLongBigEndian (this byte[] arr, int idx = 0, int size = 8)
		{
			ulong res = 0;
			for(;size-- > 0;res |= (ulong)arr[idx++] << (size * 8));
			return res;
		}

		static public ulong GetLongLittleEndian (this byte[] arr, int idx = 0, int size = 8)
		{
			ulong res = 0;
			for(int i = 0; i < size; res |= (ulong)arr[idx++] << (i++ * 8));
			return res;
		}

		static public uint GetIntBigEndian (this byte[] arr, int idx = 0, int size = 4)
		{
			uint res = 0;
			for(;size-- > 0;res |= (uint)arr[idx++] << (size * 8));
			return res;
		}

		static public uint GetIntLittleEndian (this byte[] arr, int idx = 0, int size = 4)
		{
			uint res = 0;
			for(int i = 0; i < size; res |= (uint)arr[idx++] << (i++ * 8));
			return res;
		}


	}
}

