using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;

namespace DictUtil
{
	public enum GzOS : byte
	{
		FAT,
		Amiga,
		VMS,
		Unix,
		VM_CMS,
		HPFS,
		Macintosh,
		Z_System,
		CP_M,
		TOPS20,
		NTFS,
		QDOS,
		Acorn_RISCOS,
		Unknown = 255

	}

#if !NET45
	public enum CompressionLevel
	{
		Optimal,
		Fastest,
		NoCompression,
	}
#endif

	public class GzExtraField
	{

		int _xlen;
		int _len;
		byte[] _id = new byte[2];

		internal int TotalLength { get { return _xlen + 2; } }

		internal int FieldLength { get { return _len; } }

		internal int FieldDataBegin { get { return 10 + 6; } }

		internal byte[] Id { get { return _id; } }

		internal GzExtraField (Stream fs)
		{
			_xlen = fs.ReadByte() | (fs.ReadByte() << 8);
			_id[0] = (byte)fs.ReadByte();
			_id[1] = (byte)fs.ReadByte();
			_len = fs.ReadByte() | (fs.ReadByte() << 8);
			fs.Position = TotalLength + 10;
		}

	}

	class GzAnsiString
	{
		byte[] _str = new byte[256];

		public int Length { get { return _str.Length; } }

		internal GzAnsiString (Stream fs)
		{
			int cnt = 0;
			for (int b = -1; b != 0; ++cnt)
			{
				b = fs.ReadByte();
				if (cnt >= _str.Length)
					Array.Resize(ref _str, cnt * 2);
				_str[cnt] = (byte)b;
			}
			Array.Resize(ref _str, cnt);
		}

		public override string ToString ()
		{
			return Encoding.UTF8.GetString(_str, 0, _str.Length - 1);
		}
	}

	abstract public class GZipBase<T> : IDisposable where T : GzExtraField
	{
		const int 
			GZ_ID1 = 0x1f,
			GZ_ID2 = 0x8b,
			GZ_CM = 8,
			MHCRC = 2,
			MEXTRA = 4,
			MNAME = 8,
			MCOMMENT = 16
				;
		static readonly DateTime _unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		public readonly FileMode OpenMode;
		public readonly FileStream FileStream;
		public DeflateStream DeflateStream { get; protected set;}
		bool _metaWritten = false;
		byte _flag;
		uint _mtime;
		byte _xflag;
		GzOS _os;
		GzAnsiString _name;
		GzAnsiString _comment;
		int _hcrc;
		protected long _dataBegin;
		uint _origSize;

		#region props

		public bool IsCreating { get { return OpenMode == FileMode.Create; } }

		public bool IsValid { get; private set; }

		public bool HasHeaderCRC{ get { return (_flag & MHCRC) == MHCRC; } }

		public bool HasExtraField{ get { return (_flag & MEXTRA) == MEXTRA; } }

		public bool HasName{ get { return (_flag & MNAME) == MNAME; } }

		public bool HasComment{ get { return (_flag & MCOMMENT) == MCOMMENT; } }

		public DateTime MTime{ get { return ToCLRTime(_mtime); } }

		public long OriginalSize { get { return _origSize; } }

		public long CompressedFileSize { get { return FileStream.Length; } }

		public long CompressedDataSize { get { return FileStream.Length - 8 - _dataBegin; } }

		public T ExtraField { get; protected set; }

		public string Name { get { return HasName ? _name.ToString() : ""; } }

		public string Comment{ get { return HasComment ? _comment.ToString() : ""; } }

		public int HeaderSize { get { return (int)_dataBegin; } }

		public int PrologLength { 
			get { 
				return 10 + 
					(HasExtraField ?  ExtraField.TotalLength : 0) +
					(HasName ?  _name.Length : 0) +
					(HasComment ?  _comment.Length : 0) + 
					(HasHeaderCRC ? 4 : 0); 
			} 
		}

		public string Ratio {
			get {
				return FileStream.Length <= 0xffffffff ? 
					string.Format("{0:P}", ((double)(_origSize - CompressedDataSize)) / _origSize) :
						"UNKNOWN";
			}
		}

		public GzOS OS { get { return _os; } }

		public CompressionLevel CompressionLevel{ get { return _xflag == 2 ? CompressionLevel.Optimal : CompressionLevel.Fastest; } }

		#endregion

		#region ctors

		private GZipBase (string path, FileMode openMode)
		{
			//TODO: CRC is not natively supported by .NET, therefore
			// the creation of gzip will be implemented later!
			if (openMode != FileMode.Create && openMode != FileMode.Open)
				throw new InvalidOperationException("Open mode " + openMode.ToString() + " is not supported for GZip manipulator");

			if (openMode == FileMode.Open)
			{
				FileStream = File.OpenRead(path);
				if (FileStream.Length < 18)
				{
					IsValid = false;
					Dispose();
				}

				ReadHeader();
				DeflateStream = new DeflateStream(FileStream, CompressionMode.Decompress);
			}
		}

		public GZipBase (string path) :this(path, FileMode.Open){}
		#endregion


		protected virtual void ReadHeader ()
		{
			var fs = FileStream;
			fs.Position = 0;
			int id1 = fs.ReadByte();
			int id2 = fs.ReadByte(); 
			int compressionMethod = fs.ReadByte();
				
			//magic number
			if (id1 != GZ_ID1 || id2 != GZ_ID2 || compressionMethod != GZ_CM)
			{
				IsValid = false;
				Dispose();
			}
			else
			{
				//header : 10 bytes total
				_flag = (byte)fs.ReadByte();
				_mtime = (uint)(fs.ReadByte() | (fs.ReadByte() << 8) | (fs.ReadByte() << 16) | (fs.ReadByte() << 24));
				_xflag = (byte)fs.ReadByte();
				_os = (GzOS)fs.ReadByte();

				if (HasExtraField)
					ReadExtraField();

				if (HasName)
					_name = new GzAnsiString(fs);

				if (HasComment)
					_comment = new GzAnsiString(fs);

				if (HasHeaderCRC)				
					_hcrc = fs.ReadByte() | (fs.ReadByte() << 8);


				_dataBegin = fs.Position;

//				if (_dataBegin != PrologLength)
//					throw new Exception("?" + _dataBegin.ToString() + " " + PrologLength.ToString());

				//orig size
				fs.Seek(-4, SeekOrigin.End);

				_origSize = (uint)(fs.ReadByte() | (fs.ReadByte() << 8) | (fs.ReadByte() << 16) | (fs.ReadByte() << 24));

				fs.Position = _dataBegin;
			}

		}

		abstract protected void ReadExtraField ();

		public virtual int Read (byte[] buf, int offset, int count)
		{
			int bytesRead = 0;
			for (int chuckSize = 1; bytesRead < count && chuckSize > 0;)
				bytesRead += chuckSize = DeflateStream.Read(buf, offset + bytesRead, count - bytesRead);
			return bytesRead;
		}

		public virtual byte[] ReadAllBytes ()
		{
			if (OriginalSize > 100 * 1024 * 1024)
				throw new NotSupportedException("The file is to big to read its entire content at once!");
			if (FileStream.Position != _dataBegin)
				FileStream.Position = _dataBegin;
			var buf = new byte[OriginalSize];
			Read(buf, 0, (int)OriginalSize);
			return buf;
		}

		void WriteHeader ()
		{

		}

		void WriteFooter ()
		{

		}

		#region IDisposable implementation
		public void Dispose ()
		{
			if (IsCreating && !_metaWritten)
			{
				WriteHeader();
				WriteFooter();
			}
			DeflateStream.Dispose();
		}
		#endregion


		public static DateTime ToCLRTime (long unixTime)
		{
			return _unixStartTime.AddSeconds(Convert.ToDouble(unixTime));
		}
	}

	public class GZip : GZipBase<GzExtraField>
	{
		public GZip (string path):base(path){}

		protected override void ReadExtraField ()
		{
			ExtraField = new GzExtraField(FileStream);
		}
	}
}

