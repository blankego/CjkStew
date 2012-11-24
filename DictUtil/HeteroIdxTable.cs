using System;
using System.Text;
using System.IO;

namespace DictUtil
{


	public abstract class HeteroIdxTable
	{
		readonly Func<HeteroIdxTable,int> _entryRuler;

		protected abstract int Position { set; }

		public HeteroIdxTable (Func<HeteroIdxTable,int> entryRuler)
		{
			_entryRuler = entryRuler;
		}

		public abstract byte ReadByte ();

		public abstract byte[] ReadBytes (int offset, int cnt);

		public byte[] GetEntry (int address)
		{
			Position = address;
			int cnt = _entryRuler(this);
			return ReadBytes(address, cnt);
		}

		public int FindNext (int offset)
		{
			Position = offset;
			return offset + _entryRuler(this);
		}
	}

	public class TxtHeteroIdxTable : HeteroIdxTable
	{
		readonly FileStream _fs;

		public TxtHeteroIdxTable (string filename, Func<HeteroIdxTable,int> entryRuler)
			: base(entryRuler)
		{
			_fs = File.OpenRead(filename);
		}		

		#region implemented abstract members of DictUtil.HeteroIdxTable
		protected override int Position { set {	_fs.Position = value; } }

		public override byte ReadByte ()
		{
			return (byte)_fs.ReadByte();
		}


		public override byte[] ReadBytes (int offset, int cnt)
		{
			_fs.Position = offset;
			byte[] buf = new byte[cnt];
			for (int bRead = 0,chSize = 1; bRead < cnt && chSize > 0;)
				bRead += chSize = _fs.Read(buf, bRead, cnt - bRead);
			return buf;
		}
		#endregion

	}

	public class StrHeteroIdxTable : HeteroIdxTable
	{
		readonly byte[] _tbl;
		int _startPos;

		public StrHeteroIdxTable (byte[] tbl, Func<HeteroIdxTable,int> entryRuler)
			: base(entryRuler)
		{
			_tbl = tbl;
		}

		public StrHeteroIdxTable (string gzName, Func<HeteroIdxTable,int> entryRuler) 
			: this(GZip.OpenRead(gzName).ReadAllBytes(), entryRuler){}


		#region implemented abstract members of DictUtil.HeteroIdxTable
		protected override int Position { set {	_startPos = value; } }

		public override byte ReadByte ()
		{
			return _tbl[_startPos++];
		}

		public override byte[] ReadBytes (int offset, int cnt)
		{
			byte[] buf = new byte[cnt];
			Buffer.BlockCopy(_tbl, offset, buf, 0, cnt);
			return buf;
		}
		#endregion
	}
}

