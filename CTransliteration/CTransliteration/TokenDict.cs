using System;
using System.Collections;
using System.Collections.Generic;
using SAlgo;

namespace Cjk.Phonetic
{
	public class TokenDict<T>:IEnumerable< KeyValuePair<string,T>>
	{

		class PosComparer :IComparer<string>
		{
			public int pKey, pStr;
			#region IComparer implementation
			/// <summary>
			/// Compare the specified key and str.
			/// </summary>
			/// <param name='key'>
			/// Key.
			/// </param>
			/// <param name='str'>
			/// The String to be matched from the given position with the key.
			/// </param>
			public int Compare (string key, string str)
			{

				if (key.Length <= pKey)
				{
					return pStr == str.Length ? 0 : -1; 
				}
				//Because this comparer is only used internally, bound check will be omitted
				return key [pKey].CompareTo (str [pStr]);
			}

			public PosComparer AtPos (int pK, int pS)
			{
				pKey = pK;
				pStr = pS;
				return this;
			}
			#endregion
		}

		#region fields
		PosComparer _comp = new PosComparer ();
		SortedList<string,T> _lst;
		IList<string> _keys;
		IList<T> _vals;

		#endregion fields

		public TokenDict ()
		{
			_lst = new SortedList<string, T> (StringComparer.Ordinal);
			_keys = _lst.Keys;
			_vals = _lst.Values;
		}

		public void Add (string key, T val)
		{
#if DEBUG
			if(_lst.ContainsKey(key))throw new ArgumentException(string.Format("Repeat Key:{0},{v}",key,val));
#endif
			_lst.Add (key, val);
		}

		public bool TryMatchStart (string s, ref int idx, out T val)
		{

			if (s != null)
			{
				int sLen = s.Length;
				if (idx < sLen)
				{
					int kHead = 0, KCnt = _lst.Count, candidate = -1;//Initial key range
					int kPos = 0, rollbackPos = 0;

					do
					{
						var range = _lst.Keys.EqualRange ( kHead, KCnt, s, _comp.AtPos (kPos, idx + kPos));

						if (range.From == range.To)
						{ // current char not matched  
							kPos = rollbackPos;
							break;
						}
						else
						{ //find a range 
							kHead = range.From;
							KCnt = range.To - kHead;
							string k = _keys [kHead];
							int kLen = k.Length;
							int kRestLen = kLen - kPos - 1;

							if (kRestLen == 0)
							{//reached key end, that means found a candidate
								rollbackPos = kPos + 1;
								candidate = kHead;
							}
					
							if (KCnt == 1)
							{//check the only left candidate 
								bool sliceMatch = true;
						
								//try to match the rest slice
								for (int i = 1; i <= kRestLen; ++i)
								{
									if (k [kPos + i] != s [idx + kPos + i])
									{
										sliceMatch = false;
										break;
									}
								}
								if (sliceMatch)
								{
									candidate = kHead;
									kPos += kRestLen + 1;
								}
								else
								{
									kPos = rollbackPos;
								}						
								break;
							}

						}

					} while( idx + ++kPos < sLen);

					idx += kPos;
					if (candidate >= 0)
					{
						val = _vals [candidate];
						return true;
					}
					else if (_keys [kHead].Length == 0)
					{ //empty key matches any thing! 
						val = _vals [kHead];
						return true;
					}
				}
			}
			val = default(T);
			return false;
		}



		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return ((IEnumerable<KeyValuePair<string,T>>)this).GetEnumerator ();
		}

		IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator ()
		{
			return ((IEnumerable<KeyValuePair<string,T>>)_lst).GetEnumerator ();
		}
		#endregion



	}
}

