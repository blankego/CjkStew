using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Salgo;
using System.Collections.Generic;

namespace DictUtil
{


	public class StarDict
	{
		IDictDB _db;
		IDictIDX _idx;
		StarDictInfo _info;
		Encoding _enc;
		StarDict (StarDictInfo info, IDictIDX idx, string fname, Encoding enc)
		{
			_enc = enc ?? Encoding.UTF8;
			_info = info;
			_idx = idx;
			_db = fname.EndsWith("dz") ? (IDictDB)new DictZip(fname) : (IDictDB)new TxtDictDB(fname);
		}	

		public string[] Search (string headword)
		{
			int begin, end;
			_idx.GetIndexRange(headword, out begin, out end);
			string[] res = new string[end - begin];
			for (int cnt = 0, i = begin; i < end; ++i)
			{
				long offset;
				int len;
				_idx.GetAddress(i, out offset, out len);
				res[cnt++] = _enc.GetString(_db.GetBytes(offset, len));
			}
			return res;
			//TODO: synonym file support
		}

		public static StarDict TryOpen (string basename, Encoding enc = null)
		{
			var info = new StarDictInfo(basename + ".ifo");
			if (!info.IsValid)
				return null;
			StarDictIdx idx = new StarDictIdx(info);
			return new StarDict(info, idx, basename + (File.Exists(basename + ".dict") ? ".dict" : ".dict.dz"),enc);
		}

	}
}

