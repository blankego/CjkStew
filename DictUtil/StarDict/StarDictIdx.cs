using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//using Salgo;
using System.Collections.Generic;

namespace DictUtil
{

	public class StarDictIdx : IDictIDX
	{
		Encoding _enc;
		int _offsetSize;//offset word size
		HeteroIdxTable _table;
		int[] _idxIdx; //index of index
		int _nEntries;

		int AddressSize { get { return _offsetSize + 4; } }

		public StarDictIdx (StarDictInfo info, Encoding enc = null)
		{
			_enc = enc ?? Encoding.UTF8;
			_offsetSize = info.PointerSize;
			_nEntries = info.NumberOfEntries;

			string fname = info.BaseName + ".idx";
			if (File.Exists(fname))
				_table = new TxtHeteroIdxTable(fname, MeasureEntryLength);
			else
				_table = new StrHeteroIdxTable(fname + ".gz", MeasureEntryLength);

			//create and populate the index of index
			_idxIdx = new int[_nEntries + 1];
			for (int cnt = 1, offset = 0; cnt < _nEntries; ++cnt)
			{
				_idxIdx[cnt] = offset = _table.FindNext(offset);
			}
			_idxIdx[_nEntries] = info.IndexFileSize;//last sentry
		}

		string GetHeadword (int idx)
		{
			int start = _idxIdx[idx], end = _idxIdx[idx + 1];
			var arr = _table.ReadBytes(start, end - start - AddressSize - 1);
			return _enc.GetString(arr);
		}

		int MeasureEntryLength (HeteroIdxTable tbl)
		{
			for (int cnt = 0;; ++cnt)
			{
				if (tbl.ReadByte() == 0)
					return cnt + 1 + AddressSize; // '\0' + sizeof(offsetPtr) + 4 bytes size
			}
		}

		#region IDictIDX implementation
		public bool GetAddress (int ordinal, out long offset, out int length)
		{
			if (ordinal < 0 || ordinal >= _idxIdx.Length - 1)
			{	
				offset = length = 0;
				return false;
			}
			var bs = _table.ReadBytes(_idxIdx[ordinal + 1] - AddressSize, AddressSize);
			offset = (long)bs.GetLongBigEndian(0, _offsetSize);
			length = (int)bs.GetIntBigEndian(_offsetSize);
			return true;
		}

		public bool GetIndexRange (string headword, out int begin, out int end)
		{
			begin = 0;
			end = _idxIdx.Length - 1;
			while (begin < end)
			{
				int mid = (begin + end) / 2;					
				if (CompareHW(GetHeadword(mid), headword) < 0)				
					begin = mid + 1;
				else				 
					end = mid;
			}
			end = begin;
			int hi = _idxIdx.Length - 1;
			while (end < hi)
			{
				int mid = (end + hi) / 2;					
				if (CompareHW(GetHeadword(mid), headword) <= 0)				
					end = mid + 1;
				else
					hi = mid;
			}
			return begin != end;
		}

		int CompareHW (string sa, string sb)
		{
			int rel = string.Compare(sa, sb, true);
			return rel == 0 ? string.Compare(sa, sb) : rel;
		}

		#endregion
	}
	
}
