/// <summary>
/// The files under 'GZip' directory are self-contained. You can use them to create and/or
/// decompress gzip/dzip (*.dz) files.
/// </summary>
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Collections.Generic;
using System.Runtime;

namespace DictUtil
{
	/// <summary>
	/// OS flag of gzip file, which indicates on what platform the archive is created
	/// </summary>
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
		int _len;
		byte[] _id = new byte[2];
		protected FileStream _fs;
		internal FileStream FileStream{ get{ return _fs;} set{ _fs = value; } }

		public int TotalLength { get { return XLength + 2; } }

		/// <summary>
		/// Gets the length of the whole extra field minus 2
		/// This value is redundant, will be computed from the Length field
		/// </summary>
		internal int XLength{ get { return _len + 4; } } 

		internal int Length { get { return _len; } }

		internal int FieldDataBegin { get { return 10 + 6; } }

		internal byte[] Id { get { return _id; } }

		internal virtual void ReadFrom ()
		{	
			FileStream fs = FileStream;
			//Because the xlen field if redundant its value will be discarded
			fs.GetIntLittleEndian(2);
			_id[0] = (byte)fs.ReadByte();
			_id[1] = (byte)fs.ReadByte();
			_len = (int)fs.GetIntLittleEndian(2);
			fs.Position = TotalLength + 10;
		}

		internal virtual void WriteTo ()
		{
			Stream fs = FileStream;
			//xfield header
			fs.SetIntLittleEndian((uint)XLength, 2);
			fs.Write(_id, 0, 2);
			fs.SetIntLittleEndian((uint)Length, 2);
		}
	}

	abstract public class GZipBase<T> : IDisposable where T : GzExtraField , new()
	{
		#region consts
		const int 
			GZ_ID1 = 0x1f,
			GZ_ID2 = 0x8b,
			GZ_CM = 8,
			MHCRC = 2,
			MEXTRA = 4,
			MNAME = 8,
			MCOMMENT = 16,
			BUFSIZE = 1024 * 4
				;
		#endregion

		static readonly DateTime _unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		#region fields
		public readonly FileMode OpenMode;
		public readonly FileStream FileStream;

		public DeflateStream DeflateStream { get; protected set; }

		byte _flag;
		DateTime _mtime;
		byte _xflag;
		GzOS _os;
		byte[] _name;
		byte[] _comment;
		int _hcrc;
		protected long _dataBegin;
		uint _crc = 0;
		uint _origSize;
		#endregion

		#region props

		public bool IsCreating { get { return OpenMode == FileMode.Create; } }

		public bool IsValid { get; private set; }

		public bool HasHeaderCRC{ get { return (_flag & MHCRC) == MHCRC; } }

		public bool HasExtraField{ get { return (_flag & MEXTRA) == MEXTRA; } }

		public bool HasName{ get { return (_flag & MNAME) == MNAME; } }

		public bool HasComment{ get { return (_flag & MCOMMENT) == MCOMMENT; } }

		public DateTime MTime{ get { return _mtime; } }

		public long OriginalSize { get { return _origSize; } }

		public uint Crc { get { return _crc; } }

		public long CompressedFileSize { get { return FileStream.Length; } }

		public long CompressedDataSize { get { return FileStream.Length - 8 - _dataBegin; } }

		public T ExtraField { get; protected set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <description></description>
		/// The name and comment must be specified before the write of the content, otherwise
		/// They will be left empty!
		/// <value>
		/// The name.
		/// </value>
		public string Name {
			get { return HasName ? BytesToString(_name) : ""; } 
			set { 
				if (!IsHeaderWritten && value != null && value.Length > 0)
				{
					_name = StringToBytes(value); 
					_flag |= MNAME;
				}
			}
		}

		public string Comment { 
			get { return HasComment ? BytesToString(_comment) : ""; } 
			set {
				if (!IsHeaderWritten && value != null && value.Length > 0)
				{
					_comment = StringToBytes(value); 
					_flag |= MCOMMENT;
				}
			}
		}

		public int HeaderSize { get { return (int)_dataBegin; } }

		bool IsHeaderWritten{ get; set; }

		public int PrologLength { 
			get { 
				return 10 + 
					(HasExtraField ? ExtraField.TotalLength : 0) +
					(HasName ? _name.Length : 0) +
					(HasComment ? _comment.Length : 0) + 
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
		/// <summary>
		/// Initializes a new instance of the <see cref="DictUtil.GZipBase`1"/> class.
		/// </summary>
		/// <param name='path'>
		/// Path of the archive
		/// </param>
		/// <param name='openMode'>
		/// Open mode. Only Open(read) and Create are supported
		/// </param>
		protected GZipBase (string path, FileMode openMode)
		{
			OpenMode = openMode;
			if (openMode == FileMode.Open)
			{
				FileStream = File.OpenRead(path);
				if (FileStream.Length < 18)
				{
					IsValid = false;
					Dispose();
				}

				ReadHeader();
				//subclass may change this prop, therefore, leave it open
				DeflateStream = new DeflateStream(FileStream, CompressionMode.Decompress,true);
			}
			else if (openMode == FileMode.Create)
			{
				FileStream = File.Create(path);
				DeflateStream = new DeflateStream(FileStream, CompressionMode.Compress,true);
			}
		}

		~GZipBase()
		{
			Dispose();
		}
		#endregion

		#region private helper functions
		protected void EnsureReadMode()
		{
			if(IsCreating)
				throw new InvalidOperationException("This gzip is in creating mode, read operations are not allowed!");
		}

		protected void EnsureWriteMode()
		{
			if(!IsCreating)
				throw new InvalidOperationException("This gzip is in reading mode, write operations are not allowed!");
		}

		byte[] ReadString ()
		{
			byte[] buf = new byte[256];
			int cnt = 0;
			for (int b = -1; b != 0; ++cnt)
			{
				if (cnt >= buf.Length)
					Array.Resize(ref buf, cnt * 2);
				buf[cnt] = (byte)(b = FileStream.ReadByte());
			}
			Array.Resize(ref buf, cnt);
			return buf;
		}

		string BytesToString (byte[] arr)
		{
			return Encoding.UTF8.GetString(arr, 0, arr.Length - 1);
		}

		byte[] StringToBytes (string s)
		{
			var buff = new byte[Encoding.UTF8.GetByteCount(s) + 1];
			Encoding.UTF8.GetBytes(s, 0, s.Length, buff, 0);
			return buff;
		}

		DateTime ReadUnixTime ()
		{
			uint t = 0;
			for (int i = 0; i < 4; ++i)
				t |= (uint)FileStream.ReadByte() << i * 8;
			return _unixStartTime.AddSeconds(Convert.ToDouble(t));
		}

		void WriteUnixTime ()
		{
			uint t = (uint)(DateTime.Now.Subtract(_unixStartTime).TotalSeconds);
			for (int i = 0; i < 4; ++i)			
				FileStream.WriteByte((byte)(t >> (8 * i)));

		}
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
				_mtime = ReadUnixTime();
				_xflag = (byte)fs.ReadByte();
				_os = (GzOS)fs.ReadByte();

				if (HasExtraField)
					ReadExtraField();

				if (HasName)
					_name = ReadString();

				if (HasComment)
					_comment = ReadString();

				if (HasHeaderCRC)				
					_hcrc = fs.ReadByte() | (fs.ReadByte() << 8);


				_dataBegin = fs.Position;

//				if (_dataBegin != PrologLength)
//					throw new Exception("?" + _dataBegin.ToString() + " " + PrologLength.ToString());

				//orig size
				fs.Seek(-8, SeekOrigin.End);
				_crc = fs.GetIntLittleEndian();
				_origSize = fs.GetIntLittleEndian();

				fs.Position = _dataBegin;
			}

		}

		protected virtual void ReadExtraField ()
		{
			ExtraField = new T(){FileStream = FileStream};
			ExtraField.ReadFrom();
		}

		protected virtual void WriteHeader ()
		{
			FileStream.Position = 0;
			FileStream.WriteByte(GZ_ID1);
			FileStream.WriteByte(GZ_ID2);
			FileStream.WriteByte(GZ_CM);

			FileStream.WriteByte(_flag);

			//the time stamp will be added after the compression of content is completed
			FileStream.Seek(4, SeekOrigin.Current);
			FileStream.WriteByte(4);//TODO: compression method

			byte os = 255;
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Win32NT:
				os = 0;
				break;
			case PlatformID.Unix:
				os = 3;
				break;
			case PlatformID.MacOSX:
				os = 7;
				break;
			}
			FileStream.WriteByte(os);
		
		}

		protected virtual void WriteHeaderExtraPart ()
		{
			if (HasExtraField)
				ExtraField.WriteTo();
			if (HasName)
				FileStream.Write(_name, 0, _name.Length);
			if (HasComment)
				FileStream.Write(_comment, 0, _comment.Length);
			IsHeaderWritten = true;
			_origSize = 0;
		}

		protected virtual void EnsureHeaderWritten ()
		{
			EnsureWriteMode();
			if (!IsHeaderWritten)
			{
				WriteHeader();
				WriteHeaderExtraPart();
			}
		}

		void WriteFooter ()
		{
			FileStream.Position = 4;
			WriteUnixTime();
			FileStream.Seek(0, SeekOrigin.End);
			FileStream.SetIntLittleEndian(_crc);
			FileStream.SetIntLittleEndian(_origSize);
		}


		#region API
		protected virtual int ReadTo (byte[] buf, int offset, int count)
		{
			int bytesRead = 0;
			for (int chuckSize = 1; bytesRead < count && chuckSize > 0;)
				bytesRead += chuckSize = DeflateStream.Read(buf, offset + bytesRead, count - bytesRead);
			return bytesRead;
		}

		public int Read(byte[] buf, int offset, int count)
		{
			EnsureReadMode();
			return ReadTo(buf, offset, count);
		}

		public virtual byte[] ReadAllBytes ()
		{
			EnsureReadMode();
			if (OriginalSize > 100 * 1024 * 1024)
				throw new NotSupportedException("The file is to big to read its entire content at once!");
			if (FileStream.Position != _dataBegin)
				FileStream.Position = _dataBegin;
			var buf = new byte[OriginalSize];
			ReadTo(buf, 0, (int)OriginalSize);
			return buf;
		}

		protected virtual void WriteTo (byte[] buf, int offset, int cnt)
		{
			_crc = Crc32.UpdateCrc(buf, offset, cnt, _crc);
			DeflateStream.Write(buf, offset, cnt);
			_origSize += (uint)cnt;
		}

		public void Write(byte[] buf, int offset, int cnt)
		{
			EnsureHeaderWritten();
			WriteTo(buf, offset, cnt);
		}

		public virtual void Compress (Stream s)
		{
			EnsureHeaderWritten();
			byte[] buf = new byte[BUFSIZE];
			for (int bytesRead = 0, chunkSize = 0; chunkSize > 0; bytesRead = 0)
			{
				for (; bytesRead < BUFSIZE && chunkSize > 0;)
					bytesRead += chunkSize = s.Read(buf, 0, BUFSIZE);
				WriteTo(buf, 0, bytesRead);
			}
		}

		#endregion

		#region IDisposable implementation
		public virtual void Dispose ()
		{
			if (IsCreating && FileStream != null)
			{
				EnsureHeaderWritten();
				if(DeflateStream != null)DeflateStream.Dispose();
				WriteFooter();
			}
			else
			{
				if(DeflateStream != null)DeflateStream.Dispose();
			}	
			if(FileStream != null)
				FileStream.Dispose();
		}
		#endregion



	}

	public class GZip : GZipBase<GzExtraField>
	{
		public GZip (string path, FileMode openMode):base(path, openMode){}

		static public GZip Create (string path, bool overwriteIfExists = false)
		{
			if(!overwriteIfExists && File.Exists(path))
				throw new IOException("File "+path+" already exists");
			return new GZip(path, FileMode.Create);
		}

		static public GZip OpenRead (string path)
		{
			return new GZip(path, FileMode.Open);
		}

	}
}

