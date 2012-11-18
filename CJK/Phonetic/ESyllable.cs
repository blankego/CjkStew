using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Salgo;

namespace Cjk.Phonetic
{
	public struct ESyllable
	{
		//I have to declare those fields as internal to make the populating of the tables efficient
		//Just DO NOT mess with them!
		[EntTable("syllables"), StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct _Syl
		{
			static internal StringBuilder _sb = new StringBuilder();
			internal Meld Meld;
			internal int CharsIdx;
			internal byte NChars, Initial, Final;
			internal Tone Tone;

			static internal void SetAll (ref _Syl st, int idx, string[] rec)
			{
				st.Meld = new Meld(rec[0]);
				st.CharsIdx = _sb.Length;

				//chars
				string chars = rec[1];
				_sb.Append(chars).Append('|');
				UString us = chars.ToUString();
				for (int i = 0; i != us.Length; ++i)
				{
					_charIdx.Add(us[i], (ushort)idx);
				}
				st.NChars = byte.Parse(rec[2]);
				st.Initial = byte.Parse(rec[3]);
				st.Final = byte.Parse(rec[4]);
				st.Tone = (Tone)(int.Parse(rec[5]));
			}
		}

		public struct Range : IReadOnlyList<ESyllable>
		{
			readonly ushort Begin, End;			
			internal Range(ushort begin, ushort end)
			{
				Begin = begin;
				End = end;
			}
			#region IReadOnlyList implementation
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
			{
				return GetEnumerator();
			}

			public IEnumerator<ESyllable> GetEnumerator ()
			{
				for(int i = Begin; i != End; ++i)
					yield return Get(_charIdx.GetValue(i));
			}

			public int Count { get { return End - Begin;} }

			public ESyllable this[int idx] { get { return Get(_charIdx.GetValue( Begin + idx));	} }
			#endregion


		}

		static internal readonly ArrayIndex<CodePoint,ushort> _charIdx = new ArrayIndex<CodePoint, ushort>(20000);
		static internal readonly _Syl[] Table = TableImporter<_Syl>.Import(_Syl.SetAll);
		static readonly string _allChars;

		static ESyllable ()
		{
			_allChars = _Syl._sb.ToString();
			_Syl._sb = null;
			_charIdx.Prep();
		}

		static public ESyllable Get (int idx)
		{
			if (idx < 0 || idx >= Table.Length)
				throw new ArgumentOutOfRangeException(idx.ToString());
			return new ESyllable{_i = (ushort)idx};
		}



		ushort _i;

		public EInitial Initial { get { return EInitial.Get(Table[_i].Initial); } }

		public EFinal Final { get { return  EFinal.Get(Table[_i].Final); } }

		public Tone Tone { get { return Table[_i].Tone; } }

		public ESection Section { get { return ESection.GetByRime(Table[_i].Final, Table[_i].Tone);} }

//		public T GetRime<T>() where T : IRime
//		{
//
//		}

		public UString HeadWos {
			get{
				int start = Table[_i].CharsIdx,
				 	end = _allChars.IndexOf('|',start);
				return new UString(_allChars, start, end - start);
			}
		}

		static public Range GetAll( CodePoint cp)
		{
			int b, e;
			_charIdx.EqualRange(cp, out b, out e);
			return new Range((ushort)b, (ushort)e);
		}

		static public IEnumerable<ESyllable> All{
			get{
				for(int i = 0; i != Table.Length; ++i)
					yield return Get(i);
			}
		}
	}
}

