using System;
using System.Collections.Generic;

namespace Cjk.Phonetic.Mandarin
{
	public class Bopomofo : IMTransliterator
	{
		#region dictionaries
		static TokenDict<INIT> _iDict = new TokenDict<INIT>{
			{"ㄅ", INIT.B}, {"ㄆ", INIT.P}, {"ㄇ", INIT.M}, {"ㄈ", INIT.F}, {"ㄪ", INIT.V},
			{"ㄉ", INIT.D}, {"ㄊ", INIT.T}, {"ㄋ", INIT.N}, {"ㄌ", INIT.L},
			{"ㄍ", INIT.G}, {"ㄎ", INIT.K}, {"ㄫ", INIT.NG}, {"ㄏ", INIT.H},
			{"ㄐ", INIT.G}, {"ㄑ", INIT.K}, {"ㄬ", INIT.NG}, {"ㄒ", INIT.H},
			{"ㄓ", INIT.J}, {"ㄔ", INIT.CH}, {"ㄕ", INIT.SH}, {"ㄖ", INIT.R},
			{"ㄗ", INIT.TZ}, {"ㄘ", INIT.TS}, {"ㄙ", INIT.S}
		};
		static TokenDict<MED> _mDict = new TokenDict<MED>{
			{"ㄧ", MED.I}, {"ㄨ", MED.U}, {"ㄩ", MED.IU}
		};
		static TokenDict<FIN> _fDict = new TokenDict<FIN>{
			{"ㄚ", FIN.A}, {"ㄛ", FIN.O}, {"ㄜ", FIN.E}, {"ㄝ", FIN.È}, {"ㄭ", FIN.Y}, {"ㄦ", FIN.EL},
			{"ㄞ", FIN.AI}, {"ㄟ", FIN.EI}, {"ㄠ", FIN.AU}, {"ㄡ", FIN.OU},
			{"ㄢ", FIN.AN}, {"ㄣ", FIN.EN}, {"ㄤ", FIN.ANG}, {"ㄥ", FIN.ENG}, {"ㆰ", FIN.AM}, {"ㆬ", FIN.EM}

		};
		static TokenDict<TONE> _tDict = new TokenDict<TONE>{
			{"ˉ", TONE.CLEAR}, {"ˊ", TONE.MUDDY}, {"ˇ", TONE.RISING},
			{"ˋ", TONE.GOING},
			{"·", TONE.ENTERING}, {"˙", TONE.NEUTRAL}, {"", TONE.CLEAR},

		};
		static Dictionary<INIT, string> _iTrans = new Dictionary<INIT,string>{
			{INIT.Void, ""},
			{INIT.B, "ㄅ"}, {INIT.P, "ㄆ"}, {INIT.M, "ㄇ"}, {INIT.F, "ㄈ"}, {INIT.V, "ㄪ"},
			{INIT.D, "ㄉ"}, {INIT.T, "ㄊ"}, {INIT.N, "ㄋ"}, {INIT.L, "ㄌ"},
			{INIT.G, "ㄍ"}, {INIT.K, "ㄎ"}, {INIT.NG, "ㄫ"}, {INIT.H, "ㄏ"},
			{INIT.J, "ㄓ"}, {INIT.CH, "ㄔ"}, {INIT.SH, "ㄕ"}, {INIT.R, "ㄖ"},
			{INIT.TZ, "ㄗ"}, {INIT.TS, "ㄘ"}, {INIT.S, "ㄙ"}

		};
		static Dictionary<MED, string> _mTrans = new Dictionary<MED,string>{
			{MED._, ""}, {MED.I, "ㄧ"}, {MED.U, "ㄨ"}, {MED.IU, "ㄩ"}
		};
		static Dictionary<FIN, string> _fTrans = new Dictionary<FIN,string>{
			{FIN.A, "ㄚ"}, {FIN.O, "ㄛ"}, {FIN.E, "ㄜ"}, {FIN.È, "ㄝ"}, {FIN.Y, ""}, {FIN.EL, "ㄦ"},
			{FIN.AI, "ㄞ"}, {FIN.EI, "ㄟ"}, {FIN.AU, "ㄠ"}, {FIN.OU, "ㄡ"},
			{FIN.AN, "ㄢ"}, {FIN.EN, "ㄣ"}, {FIN.ANG, "ㄤ"}, {FIN.ENG, "ㄥ"}, {FIN.AM, "ㆰ"}, {FIN.EM, "ㆬ"},
			{FIN.ZERO, ""},

		};
		static Dictionary<TONE, string> _tTrans = new Dictionary<TONE,string>{
			{TONE.CLEAR, ""}, {TONE.MUDDY, "ˊ"}, {TONE.RISING, "ˇ"}, {TONE.GOING, "ˋ"},
			{TONE.ENTERING, "·"}, {TONE.NEUTRAL, "˙"}
		};
		static Dictionary<INIT, string> _giGrp = new Dictionary<INIT,string>{
			{INIT.G, "ㄐ"}, {INIT.K, "ㄑ"}, {INIT.NG, "ㄬ"}, {INIT.H, "ㄒ"}
		};
		#endregion
		private Bopomofo ()
		{
		}

		public static Bopomofo Inst = new Bopomofo ();
		#region IMTransliterator implementation

		public MSyl 
		MunchSyllable (string s, ref int idx)
		{
			if (s == null || idx < 0 || idx >= s.Length)
				return MSyl.Default;
			int start = idx;
			INIT init;
			MED med;
			FIN fin;
			TONE tone;

			_iDict.TryMatchStart (s, ref idx, out init);
			_mDict.TryMatchStart (s, ref idx, out med);
			_fDict.TryMatchStart (s, ref idx, out fin);

			if (idx == start)
				return MSyl.Default;
			if (init >= INIT.J && fin == FIN.ZERO && med == MED._)
				fin = FIN.Y;
//			if(med != MED._ && fin == FIN.Y)//ㄧㄨㄩ as stand alone rimes
//				fin = FIN.ZERO;

			_tDict.TryMatchStart (s, ref idx, out tone);
			return new MSyl (init, med, fin, tone);
		}

		public string 
		Transcribe (MSyl syl)
		{
			var bpmf = new System.Text.StringBuilder ();
			bpmf.Append ((syl.IsPalatized && syl.InitialGroup == INITGROUP.GGRP) ? 
			            _giGrp [syl.Initial] : _iTrans [syl.Initial]
			);
			bpmf.Append (_mTrans [syl.Medial]);
			bpmf.Append (_fTrans [syl.Final]);
			bpmf.Append (_tTrans [syl.Tone]);

			return bpmf.ToString ();
		}

		public IList<AltMSyl> 
		Parse (string s)
		{
			var lst = new List<AltMSyl> ();
			int idx = 0;
			int noiseStart = -1, noiseCount = 0;
			while (idx < s.Length) {
				MSyl syl = MunchSyllable (s, ref idx);
				if (syl == MSyl.Default) {
					if (noiseStart < 0) {
						noiseStart = idx;
						noiseCount = 0;
					}
					idx++;
					noiseCount++;
				} else {
					if (noiseStart >= 0) {
						lst.Add (new AltMSyl (s.Substring (noiseStart, noiseCount)));
						noiseStart = -1;
					}
					lst.Add (new AltMSyl (syl));
				}
			}
			if (noiseStart >= 0)
				lst.Add (new AltMSyl (s.Substring (noiseStart)));
			return lst;
		}
		#endregion


	}
}

