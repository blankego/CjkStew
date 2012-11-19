using System;
using Salgo;

namespace Cjk.Phonetic
{
	public class ER
	{
		static public bool IsLevel (CodePoint cp)
		{
			var range = ESyllable.GetAllByChar(cp);
			for (int i = 0; i < range.Count; i++) 
				if (range[i].Tone == Tone.Level)				
					return true;
			return false;
		}

		static public bool IsOblique (CodePoint cp)
		{
			var range = ESyllable.GetAllByChar(cp);
			for (int i = 0; i < range.Count; i++) 
				if (range[i].Tone != Tone.Level)
					return true;				
			return false;
		}


//
//		static public T GetRime<T>( CodePoint cp) where T : IRime 
//		{
//
//		}
	}
}

