using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Salgo;
namespace Cjk.Phonetic
{
	public struct EInitial
	{

		[EntTable("initials"), StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct _Init
		{
			internal char Name;
			internal _Recons Recons;

			static internal void SetAll (ref _Init st, int idx, string[] rec)
			{
				st.Name = rec [0] [0];
				_Recons.SetAll (ref st.Recons, rec.Slice (1));
			}
		}

		static internal readonly _Init[] Table = TableImporter<_Init>.Import(_Init.SetAll);

		byte _i;

		static internal EInitial Get(int idx)
		{
			return new EInitial{_i = (byte)idx};
		}

		public byte Index{ get { return _i; } }

		public char Name{ get { return Table[_i].Name;} }

		public string Reconstruction (Reconstruction recons)
		{
			return Table[_i].Recons.Get(recons);
		}
	}
}

