using System.Runtime.InteropServices;

namespace Cjk.Phonetic.Mandarin
{
	/// <summary>
	/// Mimt : Mandarin syllable Initial, Medial, Tone combination
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
	public struct MImt
	{
		MSyl _syl;

		public INIT Initial{ get { return _syl.Initial; } set { _syl.Initial = value; } }

		public  MED Medial{ get { return _syl.Medial; } set { _syl.Medial = value; } }

		public  TONE Tone{ get { return _syl.Tone; } set { _syl.Tone = value; } }

		public override int GetHashCode ()
		{
			return _syl.GetHashCode ();
		}

		static public MImt B = new MImt (INIT.B),
			P = new MImt (INIT.P),
			M = new MImt (INIT.M),
			F = new MImt (INIT.F),
			V = new MImt (INIT.V),
			D = new MImt (INIT.D),
			T = new MImt (INIT.T),
			N = new MImt (INIT.N),
			L = new MImt (INIT.L),
			G = new MImt (INIT.G),
			K = new MImt (INIT.K),
			NG = new MImt (INIT.NG),
			H = new MImt (INIT.H),
			J = new MImt (INIT.J),
			CH = new MImt (INIT.CH),
			SH = new MImt (INIT.SH),
			R = new MImt (INIT.R),
			TZ = new MImt (INIT.TZ),
			TS = new MImt (INIT.TS),
			S = new MImt (INIT.S),
			Void = new MImt (INIT.Void);

		public MImt _ { get { return new MImt (Initial, MED._, Tone); } }

		public MImt I { get { return new MImt (Initial, MED.I, Tone); } }

		public MImt U { get { return new MImt (Initial, MED.U, Tone); } }

		public MImt IU { get { return new MImt (Initial, MED.IU, Tone); } }

		public MImt MAGIC { get { return new MImt (Initial, MED.MAGIC, Tone); } }

		public MImt c { get { return new MImt (Initial, Medial, TONE.CLEAR); } }

		public MImt m { get { return new MImt (Initial, Medial, TONE.MUDDY); } }

		public MImt r { get { return new MImt (Initial, Medial, TONE.RISING); } }

		public MImt g { get { return new MImt (Initial, Medial, TONE.GOING); } }

		public MImt e { get { return new MImt (Initial, Medial, TONE.ENTERING); } }

		public MImt n { get { return new MImt (Initial, Medial, TONE.NEUTRAL); } }

		public MImt (INIT i, MED m = MED._, TONE t = TONE.NEUTRAL)
		{
			_syl = new MSyl(i,m,FIN.ZERO,t);
		}

		public MImt (MSyl syl)
		{
			_syl = syl;
		}
	}
}
