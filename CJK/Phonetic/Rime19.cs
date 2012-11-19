using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using Salgo;
namespace Cjk.Phonetic
{
	public struct Rime19 : IRime,IEquatable<Rime19>
	{
		[EntTable("rime19"), StructLayout(LayoutKind.Sequential,Pack = 1)]
		internal struct _Sect
		{
			internal byte Volume, Ordinal;

			internal static void SetAll (ref _Sect st, int idx, IList<string> rec)
			{
				st.Volume = byte.Parse(rec[0].Substring(0, 1));
				st.Ordinal = byte.Parse(rec[0].Substring(2, 2));
				foreach(var ch in rec[1])
				{
					int i = 0;
					for(; i != ESection.Table.Length; ++i)
						if(ESection.Table[i].Name == ch)
							break;
					_19ToEr.Add((byte)idx,(byte)i);
					_erTo19[i].Add((byte)idx);
				
				}

			}
		}

		[StructLayoutAttribute(LayoutKind.Sequential,Pack = 1)]
		internal struct Fork
		{
			byte _first, _sencond;
			public byte  First{ get { return _first;} }
			public byte Second{ get { return _sencond; } }
			public int Count{ get { return First == Second ? 0 : Second == 0xFF ? 1 : 2; } }
			public void Add(byte i)
			{
				switch (Count)
				{
				case 1:
					_sencond = i;
					break;
				default://assume the first
					_first = i;
					_sencond = 0xFF;
					break;				
				}
			}
		}

		static internal readonly ArrayIndex<byte,byte> _19ToEr = new ArrayIndex<byte, byte>();
		static internal readonly Fork[] _erTo19 = new Fork[206];
		static internal readonly _Sect[] Table = TableImporter<_Sect>.Import(_Sect.SetAll);
		static internal readonly HashSet<CodePoint> _jias = new HashSet<CodePoint>(
			"佳涯娃哇洼媧緺騧蝸蛙卦挂詿罣畫絓".IterCodePoints(0)
		);

		static Rime19()
		{
			_19ToEr.Prep();
		}

		byte _i;
	
		public IEnumerable<ESection> ESections {
			get{
				int b, e;
				_19ToEr.EqualRange(_i,out b, out e);
				for(int i = b; i != e; ++i)
					yield return ESection.Get(_19ToEr.GetValue(i));
			}
		}
		public int Volume { get { return Table[_i].Volume; } }

		public int Ordinal { get { return Table[_i].Ordinal; } }

		public Tone Tone { get { return (Tone)(Volume == 1 ? 1 : Volume - 1); } }

		public char Title { get { return (char)(_i + 0x2460); } }

		public bool Contains(CodePoint cp)
		{
			return GetAllByChar(cp).Any(Equals);
		}

		internal Fork FromEr(ESection er)
		{
			return _erTo19[er.Index];
		}

		public RimeLeague<Rime19> League {
			get {
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ESyllable> Syllables {
			get {
				return ESections.SelectMany(es=>es.Syllables);;
			}
			
		}


		static public IEnumerable<Rime19> GetAllByChar(CodePoint cp)
		{
			var range = ESyllable.GetAllByChar(cp);
			Rime19 last = new Rime19{_i = 0xff};
			for (int i = 0; i < range.Count; ++i)
			{
				var syl = range[i];
				var er = ESection.GetByRime(syl.Final.Index,syl.Tone);
				var fork = _erTo19[er.Index];
				var curr = 	new Rime19{_i = (fork.First == 30 /*泰*/&& !syl.Final.Rounded) ? fork.Second : fork.First};

				if(!curr.Equals(last))
				{

					yield return last = curr;
					if(fork.Count == 2 && (er.Title == '佳' || er.Title == '卦') && _jias.Contains(cp))
					{
						yield return last = new Rime19{_i = fork.Second};
					}
				}
			}
		}

		public bool Equals(Rime19 other)
		{
			return _i == other._i;
		}
	}
}

