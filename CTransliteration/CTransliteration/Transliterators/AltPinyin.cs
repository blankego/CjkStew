using System;
using System.Collections.Generic;
using System.Text;
namespace Cjk.Phonetic.Mandarin
{
	/// <summary>
	/// Another frequently used Pinyin notation that employs numbers as an alternative of tone mark sytem 
	/// 
	/// The numbers from 0 to 5 represent entering, clear, muddy, rising, going and neutral respectively
	/// </summary>
	public class AltPinyin: Pinyin, IMTransliterator
	{
		#region dictionaries


		static TokenDict<TONE> _tDict  = new TokenDict<TONE>{
			{"ˉ",TONE.CLEAR},{"´",TONE.MUDDY},{"ˇ",TONE.RISING},{"`",TONE.GOING},
			{"·",TONE.ENTERING},{"˙",TONE.NEUTRAL},{"",TONE.NEUTRAL},
			{"1",TONE.CLEAR},{"2",TONE.MUDDY},{"3",TONE.RISING},{"4",TONE.GOING},
			{"0",TONE.ENTERING},{"5",TONE.NEUTRAL},
			
		};


		static Dictionary<TONE,string> _tTrans = new Dictionary<TONE,string>{
			{TONE.CLEAR,"1"},{TONE.MUDDY,"2"},{TONE.RISING,"3"},{TONE.GOING,"4"},
			{TONE.ENTERING,"0"},{TONE.NEUTRAL,"5"}
		};
		#endregion

		public new static readonly AltPinyin Inst = new AltPinyin();
		public AltPinyin ()
		{
		}
		#region IMTransliterator implementation
		public override MSyl 
		MunchSyllable (string s, ref int idx)
		{
			MSyl syl = base.MunchSyllable(s,ref idx);
			TONE t;
			if(_tDict.TryMatchStart(s,ref idx,out t))syl.Tone = t;
			return syl;

		}

		public override IList<AltMSyl> 
		Parse (string s)
		{
			throw new System.NotImplementedException ();
		}

		public override string 
		Transcribe (MSyl syl)
		{
			string t = _tTrans[ syl.Tone];
			syl.Tone = TONE.NEUTRAL;
			string s = base.Transcribe(syl);
			return s + t;

		}
		#endregion

	}
}

