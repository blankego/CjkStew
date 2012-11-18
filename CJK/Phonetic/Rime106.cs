using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Salgo;

namespace Cjk.Phonetic
{
	public struct Rime106 : IRime
	{
		[EntTable("rime106"), StructLayout(LayoutKind.Sequential,Pack = 1)]
		internal struct _Sect
		{
			internal byte Volume, oinal;
			internal char Name;

			internal static void SetAll (ref _Sect st, int idx, IList<string> rec)
			{
				st.Volume = byte.Parse(rec[0].Substring(0, 1));
				st.oinal = byte.Parse(rec[0].Substring(2, 2));
				st.Name = rec[0][4];
				foreach (var o in rec.Skip(1).Select(it=>byte.Parse(it)))
				{
					int i = 0;
					for (; i != ESection.Table.Length; ++i)
						if (ESection.Table[i].Volume == st.Volume && ESection.Table[i].oinal == o)
							break;
					_106ToEr.Add((byte)idx, (byte)i);
					_erTo106[i] = (byte)idx;
				}

			}
		}
		static internal readonly ArrayIndex<byte,byte> _106ToEr = new ArrayIndex<byte, byte>();
		static internal readonly byte[] _erTo106 = new byte[206];
		static internal readonly _Sect[] Table = TableImporter<_Sect>.Import(_Sect.SetAll);

		static Rime106 ()
		{
			_106ToEr.Prep();
		}

		byte _i;

		static internal Rime106 Get (int idx)
		{
			return new Rime106{_i = (byte)idx};
		}

		public IEnumerable<ESection> ESections {
			get {
				int b, e;
				_106ToEr.EqualRange(_i, out b, out e);
				for (int i = b; i != e; ++i)
					yield return ESection.Get(_106ToEr.GetValue(i));
			}
		}

		public int Volume { get { return Table[_i].Volume; } }

		public int Ordinal { get { return Table[_i].oinal; } }

		public Tone Tone { get { return (Tone)(Volume == 1 ? 1 : Volume - 1); } }

		public char Title { get { return Table[_i].Name; } }

		public bool Contains (CodePoint cp)
		{
			throw new NotImplementedException();
		}

		public int AdjustedOrdinal {
			get {
				int vol = Volume, o = Ordinal, ao;
				switch (vol)
				{
				case 1:
					ao = o < 9 ? o : o + 1;
					break;
				case 2:
					ao = o + 16;
					break;
				case 3:
					ao = o < 9 ? o : o < 25 ? o + 1 : o + 2;
					break;
				case 4:
					ao = o < 26 ? o : o + 1;
					break;
				default:
					ao = o < 4 ? o : o < 10 ? o + 8 : o < 14 ? o + 13 : o + 14;
					break;
				}
				return ao;
			}
		}

		public SectionLeague<Rime106> League {
			get {
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ESyllable> Syllables {
			get {
				return ESections.SelectMany(es => es.Syllables);
			}
			
		}

	}



}

