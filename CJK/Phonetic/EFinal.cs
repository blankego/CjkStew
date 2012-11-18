using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Salgo;
using System.Linq;

namespace Cjk.Phonetic
{
	public struct EFinal
	{		
		[EntTable("finals"), StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct _Fin
		{
			internal char Group;
			internal bool Rounded;
			internal byte Grade;
			internal string Name;
			internal bool Clicked;
			internal _Recons Recons;

			static internal void SetAll (ref _Fin st, int idx, string[] rec)
			{
				st.Group = rec[0][0];
				st.Rounded = rec[1] == "1";
				st.Grade = (byte)int.Parse(rec[2]);
				st.Name = rec[3];
				st.Clicked = rec[4] == "1";
				_Recons.SetAll(ref st.Recons, rec.Slice(5));
			}
		}

		internal byte Index{ get { return _i;} }

		static internal readonly _Fin[] Table = TableImporter<_Fin>.Import(_Fin.SetAll);
		byte _i;

		static internal EFinal Get (int idx)
		{
			return new EFinal{_i = (byte)idx};
		}

		public string Reconstruction (Reconstruction recons)
		{
			return Table[_i].Recons.Get(recons);
		}

		public string  Name { get { return Table[_i].Name; } }

		public char Group { get { return Table[_i].Group; } }

		public byte Grade { get { return Table[_i].Grade; } }

		public bool Clicked { get { return Table[_i].Clicked; } }

		public IEnumerable<ESyllable> Syllables{
			get{
				byte idx = _i;
				return ESyllable.All.Where(syl=>syl.Final.Index == idx);
			}
		}
	}
}

