using System;
using System.IO;
using System.Text;

namespace DictUtil
{
	public class TxtDictDB : IDictDB
	{
		readonly FileStream _fs;
		readonly Encoding _enc;

		public TxtDictDB (string fname, Encoding enc = null)
		{
			_enc = enc ?? Encoding.UTF8;
			_fs = File.OpenRead(fname);
		}

		#region IDictDB implementation
		public string GetEntry (long offset, int length)
		{
			return _enc.GetString(GetBytes(offset, length));
		}

		public byte[] GetBytes (long offset, int length)
		{
			_fs.Position = offset;
			byte[] buf = new byte[length];
			for (int bytesRead = 0, chunkSize = 1; chunkSize > 0;)
				bytesRead += chunkSize = _fs.Read(buf, bytesRead, length - bytesRead);
			return buf;
		}
		#endregion
		~TxtDictDB ()
		{
			_fs.Dispose();
		}
	}
}

