using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Salgo;

namespace Cjk.Phonetic
{
	public struct Rime106 : IRime, IEquatable<Rime106>
	{
		[EntTable("rime106"), StructLayout(LayoutKind.Sequential,Pack = 1)]
		internal struct _Sect
		{
			internal byte Volume, Ordinal;
			internal char Name;

			internal static void SetAll (ref _Sect st, int idx, IList<string> rec)
			{
				st.Volume = byte.Parse(rec[0].Substring(0, 1));
				st.Ordinal = byte.Parse(rec[0].Substring(2, 2));
				st.Name = rec[0][4];
				foreach (var o in rec.Skip(1).Select(it=>byte.Parse(it)))
				{
					int i = 0;
					for (; i != ESection.Table.Length; ++i)
						if (ESection.Table[i].Volume == st.Volume && ESection.Table[i].Ordinal == o)
							break;
					_106ToEr.Add((byte)idx, (byte)i);
					_erTo106[i] = (byte)idx;
				}
				_titleDict[st.Name] = (byte)idx;
			}
		}

		#region static fields
		static internal readonly ArrayIndex<byte,byte> _106ToEr = 
			new ArrayIndex<byte, byte>(206, (a,b) => {
				var rel = a.Key.CompareTo(b.Key);
				return rel == 0 ? a.Value.CompareTo(b.Value) : rel;
			}
			);
		static internal readonly Dictionary<char,byte> _titleDict = new Dictionary<char, byte>(106);
		static internal readonly byte[] _erTo106 = new byte[206];
		static internal readonly _Sect[] Table = TableImporter<_Sect>.Import(_Sect.SetAll);

		static Rime106 ()
		{
			_106ToEr.Prep();
		}
		#endregion

		byte _i;

		static internal Rime106 Get (int idx)
		{
			return new Rime106{_i = (byte)idx};
		}

		static public Rime106 GetByTitle(char title)
		{
			byte idx;
			if(!_titleDict.TryGetValue(title,out idx))
				throw new ArgumentException(title.ToString() + " is not a valid rime name!");
			return new Rime106{_i = idx};
		}

		static public IEnumerable<Rime106> GetAllByChar (CodePoint cp)
		{
			Rime106 last = new Rime106{_i = 0xFF};
			foreach (var er in ESection.GetAllByChar(cp))
			{
				Rime106 curr = Get(_erTo106[er.Index]);
				if (!curr.Equals(last))
				{
					last = curr;
					yield return curr;
				}
			}
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

		public int Ordinal { get { return Table[_i].Ordinal; } }

		public Tone Tone { get { return (Tone)(Volume == 1 ? 1 : Volume - 1); } }

		public char Title { get { return Table[_i].Name; } }

		public bool Contains (CodePoint cp)
		{
			return GetAllByChar(cp).Any(Equals);
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

		public Rime106? Peer (Tone tone)
		{
			int ao = AdjustedOrdinal;
			int id;
			switch (tone)
			{
			case Tone.Level:
				id = ao < 9 ? ao : ao > 9 ? ao - 1 : 0;
				break;
			case Tone.Rising:
				id = ao < 9 ? ao + 30 : 9 < ao && ao < 26 ? ao + 29 : ao > 26 ? ao + 28 : 0;
				break;
			case Tone.Going:
				id = ao < 26 ? ao + 59 : ao > 26 ? ao + 58 : 0;
				break;
			default:
				id = ao < 4 ? ao + 89 :
					     11 < ao && ao < 18 ? ao + 81 : 22 < ao && ao < 27 ? ao + 76 : ao > 27 ? ao + 75 : 0;
				break;
			}
			if (id == 0)
				return null;
			else
				return new Rime106{_i = (byte)(id - 1)};
		}

		public RimeLeague<Rime106> League {
			get {
				return new RimeLeague<Rime106>(
					Peer(Tone.Level), Peer(Tone.Rising), Peer(Tone.Going), Peer(Tone.Entering)
				);
			}
		}

		public IEnumerable<ESyllable> Syllables {
			get {
				return ESections.SelectMany(es => es.Syllables);
			}
			
		}

		public bool Equals (Rime106 other)
		{
			return _i == other._i;
		}

		public override int GetHashCode ()
		{
			return _i;
		}
	}



}

