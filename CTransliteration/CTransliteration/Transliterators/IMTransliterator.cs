using System;
using System.Collections.Generic;
namespace Cjk.Phonetic.Mandarin
{
	public interface IMTransliterator
	{
		MSyl MunchSyllable(string s, ref int idx);
		IList<AltMSyl> Parse(string s);
		string Transcribe(MSyl syl);
	}

	public struct AltMSyl
	{
		public readonly MSyl MSyl;
		public readonly string Noise;
		public AltMSyl(string noise){Noise = noise; MSyl = MSyl.Default;}
		public AltMSyl(MSyl syl){MSyl = syl; Noise = null;}
		public bool IsMSyl {
			get { return Noise == null;}
		}
		public override string ToString ()
		{
			return IsMSyl ? MSyl.ToString() : Noise;
		}
	}
}

