using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using Salgo;
namespace Cjk.Phonetic
{
	public struct Rime19 : IRime
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
					_erTo19[i] = (byte)idx;
				}

			}
		}
		static internal readonly ArrayIndex<byte,byte> _19ToEr = new ArrayIndex<byte, byte>();
		static internal readonly byte[] _erTo19 = new byte[206];
		static internal readonly _Sect[] Table = TableImporter<_Sect>.Import(_Sect.SetAll);
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
			throw new NotImplementedException();
		}
		public SectionLeague<Rime19> League {
			get {
				throw new NotImplementedException();
			}
		}

		public IEnumerable<ESyllable> Syllables {
			get {
				return ESections.SelectMany(es=>es.Syllables);;
			}
			
		}
	}
}

