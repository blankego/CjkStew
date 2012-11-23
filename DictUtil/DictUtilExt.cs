using System;
using System.IO;

namespace DictUtil
{
	public static class DictUtilExt
	{
		static public ulong GetUlongBigEndian (this Stream s)
		{

			return 	(ulong)s.ReadByte() << 56 |
				(ulong)s.ReadByte() << 48 |
				(ulong)s.ReadByte() << 40 |
				(ulong)s.ReadByte() << 32 |
				(ulong)s.ReadByte() << 24 |
				(ulong)s.ReadByte() << 16 |
				(ulong)s.ReadByte() << 8 |
				(byte)s.ReadByte();		
		}

		static public uint GetUintBigEndian (this Stream s)
		{
			return (uint)s.ReadByte() << 24 |
				(uint)s.ReadByte() << 16 |
				(uint)s.ReadByte() << 8 |
				(uint)s.ReadByte();
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

