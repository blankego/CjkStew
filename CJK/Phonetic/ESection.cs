using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Salgo;

namespace Cjk.Phonetic
{
	public struct RimeLeague<T> where T: struct, IRime
	{
		internal T? _level, _rising, _going, _entering;

		public T? Level{ get { return _level; } }

		public T? Rising{ get { return _rising; } }

		public T? Going{ get { return _going; } }

		public T? Entering{ get { return _entering; } }

		internal RimeLeague(T? level, T? rising, T? going, T? entering)
		{
			_level = level;
			_rising = rising;
			_going = going;
			_entering = entering;
		}
	}

	public interface IRime
	{
		char Title{ get; }

		Tone Tone{ get; }

		int Volume{ get; }

		int Ordinal{ get; }

		IEnumerable<ESyllable> Syllables{ get; }

		bool Contains (CodePoint cp);

	}

	public struct ESection : IRime,IEquatable<ESection>
	{
		[EntTable("toc"), StructLayout(LayoutKind.Sequential,Pack = 1)]
		internal struct _Sect
		{
			internal byte Volume, Ordinal;
			internal char Name;

			internal static void SetAll (ref _Sect st, int idx, IList<string> rec)
			{
				st.Volume = byte.Parse(rec[0].Substring(0, 1));
				st.Ordinal = byte.Parse(rec[0].Substring(2, 2));
				st.Name = rec[0][4];
			}
		}

		[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
		internal struct FinTon : IComparable<FinTon>
		{
			internal byte Final;
			internal Tone Tone;

			public int CompareTo (FinTon other)
			{
				int rel = (byte)Tone - (byte)other.Tone;
				return rel == 0 ? Final - other.Final : rel;
			}
		}

		[EntTableAttribute("toc2rimes"), StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
		internal struct _SectMap
		{
			internal byte Sect, Final;
			internal Tone Tone;

			internal static void SetAll (ref _SectMap st, int idx, string[] rec)
			{
				st.Sect = byte.Parse(rec[0]);
				st.Final = byte.Parse(rec[1]);
				st.Tone = (Tone)byte.Parse(rec[2]);
			}

			internal _SectMap (byte sect, byte fin, Tone tone)
			{
				Final = fin;
				Tone = tone;
				Sect = sect;
			}
		}
		static void SetSect2Rimes (ref KeyValuePair<byte,FinTon> st, int idx, string[] rec)
		{
			st = new KeyValuePair<byte, FinTon>(
				byte.Parse(rec[0]),
				new FinTon{Final = byte.Parse(rec[1]), Tone = (Tone)byte.Parse(rec[2]) }
			);
		}

		static int CompareByRime (_SectMap a, _SectMap b)
		{
			int rel = (byte)a.Tone - (byte)b.Tone;
			return rel == 0 ? a.Final - b.Final : rel;
		}

		static int CompareBySect (_SectMap a, _SectMap b)
		{
			return a.Sect.CompareTo(b.Sect);
		}

		static internal readonly _Sect[] Table = TableImporter<_Sect>.Import(_Sect.SetAll);
		static internal readonly ArrayIndex<byte,FinTon> Sect2Rimes = 
			new ArrayIndex<byte, FinTon>(TableImporter<KeyValuePair<byte,FinTon>>.Import(SetSect2Rimes, "toc2rimes"));
		static internal readonly ArrayIndex<FinTon,byte> Rime2Sect =
			new ArrayIndex<FinTon, byte>(Sect2Rimes.Flip().OrderBy(kv => kv.Key).ToArray());
		static internal readonly Dictionary<char,byte> Title2Sect = new Dictionary<char, byte>();

		static ESection ()
		{
			for (int i = 0; i != Table.Length; ++ i)
				Title2Sect[Table[i].Name] = (byte)i;
		}

		byte _i;

		static internal ESection Get (int idx)
		{
			return new ESection{_i = (byte)idx};
		}

		internal int Index { get { return _i; } }

		public int Volume { get { return Table[_i].Volume; } }

		public int Ordinal { get { return Table[_i].Ordinal; } }

		public Tone Tone { get { return (Tone)(Volume == 1 ? 1 : Volume - 1); } }

		public char Title { get { return Table[_i].Name; } }

		public RimeLeague<ESection> League {
			get {
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ESyllable> Syllables {
			get {
				return Sect2Rimes.EqualRange((byte)_i)
				.SelectMany(m => ESyllable.All.Where(s => s.Final.Index == m.Final && s.Tone == m.Tone));
			}
			
		}

		public Rime106 Rime106 {
			get {
				return Rime106.Get(Rime106._erTo106[_i]);
			}
		}

		public Rime19? Rime19 {
			get {
				//TODO
				throw new NotImplementedException();

			}
		}

		public bool Contains (CodePoint cp)
		{
			bool test = false;
			var r = ESyllable.GetAllByChar(cp);
			int cnt = r.Count;
			if (cnt > 0)			
				for (int i = 0; i < cnt; i++)
				{
					if (r[i].Section._i == _i)
					{
						test = true;
						break;
					}
				}
			return test;

		}

		static internal ESection GetByRime (byte fin, Tone tone)
		{
			int idx = Rime2Sect.BinarySearch(new FinTon{Final = fin,Tone = tone});
			return Get(Rime2Sect.GetValue(idx));
		}

		static public ESection GetByTitle (char title)
		{
			byte idx;
			if (!Title2Sect.TryGetValue(title, out idx))
				throw new ArgumentOutOfRangeException();
			return Get(idx);
		}

		static public IEnumerable<ESection> GetAllByChar (CodePoint cp)
		{
			var r =  ESyllable.GetAllByChar(cp);
			ESection lastSect = new ESection{_i = 0xFF};
			for (int i = 0; i < r.Count; ++i)
			{
				var syl = r[i];
				var currSect = GetByRime(syl.Final.Index,syl.Tone);
				if(!currSect.Equals(lastSect))
				{
					lastSect = currSect;
					yield return currSect;
				}
			}
		}

		public bool Equals(ESection other)
		{
			return _i == other._i;
		}
		public override int GetHashCode ()
		{
			return _i;
		}
	}
}

