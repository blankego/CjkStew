using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Cjk.Phonetic.Mandarin
{
	/// <summary>
	/// M rt. Mandarin rime + tone combination
	/// </summary>
	[StructLayout(LayoutKind.Sequential,Pack=1,Size=2)]
	public struct MRt:IEquatable<MRt>
	{
		MSyl _syl;

		public  MED Medial{ get { return _syl.Medial; } set { _syl.Medial = value; } }

		public  FIN Final{ get { return _syl.Final; } set { _syl.Final = value; } }

		public  TONE Tone{ get { return _syl.Tone; } set { _syl.Tone = value; } }

		public RIME Rime{ get { return _syl.Rime; } set { _syl.Rime = value; } }

		public override int GetHashCode ()
		{
			return _syl.GetHashCode ();
		}

		static public readonly 
			MRt
			A = new MRt (FIN.A),
			ZERO = new MRt (FIN.ZERO),
			O = new MRt (FIN.O),
			E = new MRt (FIN.E),
			EH = new MRt (FIN.È),
			EL = new MRt (FIN.EL),
			Y = new MRt (FIN.Y),
			AI = new MRt (FIN.AI),
			EI = new MRt (FIN.EI),
			AU = new MRt (FIN.AU),
			OU = new MRt (FIN.OU),
			AN = new MRt (FIN.AN),
			EN = new MRt (FIN.EN),
			ANG = new MRt (FIN.ANG),
			ENG = new MRt (FIN.ENG),
			AM = new MRt (FIN.AM),
			EM = new MRt (FIN.EM),
			ONG = new MRt (FIN.ENG, MED.U);

		static public  readonly MRt Default = ZERO._.c;

		public MRt _ { get { return new MRt (Final, MED._, Tone); } }

		public MRt I { get { return new MRt (Final, MED.I, Tone); } }

		public MRt U { get { return new MRt (Final, MED.U, Tone); } }

		public MRt IU { get { return new MRt (Final, MED.IU, Tone); } }

		public MRt MAGIC { get { return new MRt (Final, MED.MAGIC, Tone); } }

		public MRt c { get { return new MRt (Final, Medial, TONE.CLEAR); } }

		public MRt m { get { return new MRt (Final, Medial, TONE.MUDDY); } }

		public MRt r { get { return new MRt (Final, Medial, TONE.RISING); } }

		public MRt g { get { return new MRt (Final, Medial, TONE.GOING); } }

		public MRt e { get { return new MRt (Final, Medial, TONE.ENTERING); } }

		public MRt n { get { return new MRt (Final, Medial, TONE.NEUTRAL); } }

		public MRt (FIN fin, MED med = MED._, TONE tone = TONE.NEUTRAL)
		{
			_syl = new MSyl (INIT.Void, med, fin, tone);
		}

		public MRt (RIME rime, TONE tone)
		{
			_syl = new MSyl ((byte)tone, (byte)rime);
		}

		public MRt (FIN fin, TONE tone):this(fin,MED._,tone)
		{
		}		
		#region IEquatable implementation
		public bool Equals (MRt other)
		{
			return this._syl.Equals (other._syl);
		}
		#endregion


	}
}

