using System;
using System.Collections;
using System.Collections.Generic;

namespace SAlgo
{
	public struct IntRange:IEnumerable<int>,IEnumerable
	{
		public readonly int From, To;

		public IntRange (int f, int t){
			From = f;
			To = t;}

		public IEnumerator<int> GetEnumerator()
		{
			for (int i = From; i != To; ++i)
				yield return i;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	static public class ListEx
	{
		static public IntRange __(this int start, int end)
		{
			return new IntRange(start, end);
		}

		static public int LowerBound<T>(this IList<T> lst, int idx, int len, T val,
		                                 IComparer<T> comp, out int rel)
		{

			if (len < 0)
			{
				throw new ArgumentOutOfRangeException("Length is less than 0!");
			} else
			{
				rel = 0;
			}
			//To speed up the computaion all other validity checks are omitted
			while (len > 0)
			{
				int step = len / 2;
				int mid = idx + step;
				rel = comp.Compare(lst[mid], val);
				if (rel < 0)
				{
					idx = ++mid;
					len -= step + 1;
				} else
				{ 
					len = step;
				}
			}
			return idx;			
		}

		static public int LowerBound<T>(this IList<T> lst, int idx, int len, T val, IComparer<T> comp)
		{
			int rel;
			return lst.LowerBound(idx, len, val, comp, out rel);
		}
			
		static public int UpperBound<T>(this IList<T> lst, int idx, int len, T val, IComparer<T> comp)
		{
			if (len < 0)
			{
				throw new ArgumentOutOfRangeException("Length is less than 0!");
			}
			while (len > 0)
			{
				int step = len / 2;
				int mid = idx + step;
				if (comp.Compare(lst[mid], val) <= 0)
				{
					idx = ++mid;
					len -= step + 1;
				} else
				{
					len = step;
				}
			}
			return idx;
		}
		static public IntRange EqualRange<T>(this IList<T> lst, int idx, int len, T val, IComparer<T> comp)
		{
			int lo = lst.LowerBound(idx, len, val, comp);
			int diff = len - lo;
			int hi = diff > 0 ? lst.UpperBound(lo, len - lo, val, comp) : lo;
			return new IntRange(lo,hi);
		}

	}
}

