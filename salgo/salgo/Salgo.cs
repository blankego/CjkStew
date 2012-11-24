using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Salgo
{
	#region Slice
	public interface IRange
	{
		int Begin{ get; }

		int End{ get; }
	}
#if !NET45
	public interface IReadOnlyList<T> : IEnumerable<T>
	{
		int Count { get; }

		T this [int idx] {get;
		}
	}


#endif
	public interface ISlice<T> : IRange,IReadOnlyList<T>,IEnumerable<T>
	{
	}

	public class ListSlice<T> : ISlice<T> ,IEnumerator<T>
	{
		static public readonly ListSlice<T> Empty = new ListSlice<T>(new T[0]);
		int _idx;

		public ListSlice (IList<T> list, int begin, int end)
		{
			List = list; 
			Begin = begin;
			End = end;
		}

		public ListSlice (IList<T> list, int begin = 0):this(list, begin, list.Count) {}

		#region interface implementation
		public IList<T> List { get; private set; }

		public int Begin { get; private set; }

		public int End { get; private set; }

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			Reset();
			return this;
		}

		public int Count { get { return End - Begin; } }

		public T this [int idx] {
			get {
				return List[Begin + idx];
			}
		}

		public void Dispose ()
		{
			Reset();
		}

		public bool MoveNext ()
		{
			return ++_idx < End;
		}

		public void Reset ()
		{
			_idx = Begin - 1;
		}

		public T Current { get { return List[_idx]; } }

		Object IEnumerator.Current { get { return Current; } }
		#endregion


	}

	public class ArraySlice<T> :  ISlice<T>, IEnumerator<T>
	{
		static public readonly ArraySlice<T> Empty = new ArraySlice<T>(new T[0], 0, 0);
		int _idx;

		public  ArraySlice (T[] arr, int begin, int end) 
		{
			Array = arr;
			Begin = begin;
			End = end;
		}

		public ArraySlice (T[] arr, int begin = 0) : this(arr, begin, arr.Length) {}



		#region interface implementation
		public T[] Array { get; private set; }

		public int Begin { get; private set; }

		public int End { get; private set; }

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator ()
		{
			Reset();
			return this;
		}

		public int Count { get { return End - Begin; } }

		public T this [int idx] {
			get {
				return Array[Begin + idx];
			}
		}

		public void Dispose ()
		{
			Reset();
		}

		public bool MoveNext ()
		{
			return ++_idx < End;
		}

		public void Reset ()
		{
			_idx = Begin - 1;
		}

		public T Current { get { return Array[_idx]; } }

		Object IEnumerator.Current { get { return Current; } }
		#endregion


		public void VisitAt (int i, RefAction<T> visitor)
		{
			visitor(ref Array[Begin + i]);
		}
	}

	public class AdaptorSlice<T,U>: ISlice<U>
	{
		static public readonly AdaptorSlice<T,U> Empty = new AdaptorSlice<T, U>(ArraySlice<T>.Empty, (T a) => default(U));
		readonly ISlice<T> _slice;
		readonly Func<T,U> _adaptor;

		public AdaptorSlice (ISlice<T> slice, Func<T,U> adaptor) 
		{
			_slice = slice;
			_adaptor = adaptor;
		}	

		#region interface implementation
		public U this [int idx] { 
			get { return _adaptor(_slice[idx]); } 
		}

		public int Count{ get { return _slice.Count; } }

		public int Begin{ get { return _slice.Begin; } }

		public int End{ get { return _slice.End; } }

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator<U> GetEnumerator ()
		{
			for (int i = 0; i != Count; ++i)
				yield return _adaptor(_slice[i]);
		}
		#endregion

	}
	#endregion

	public delegate void RefAction<T> (ref T o);

	#region Comparison by reference



	public delegate int RefComparison<T> (ref T a,ref T b);

	public interface IRefComparer<T>
	{
		int Compare (ref T a, ref T b);
	}

	public interface IRefComparable<T>
	{
		int CompareTo (ref T other);
	}

	public abstract class RefComparer<T> : IRefComparer<T> where T: IRefComparable<T>
	{
		static public readonly RefComparer<T> Default = new Inner();
		static public readonly RefComparison<T> Comparison = RefComparer<T>.Default.Compare;

		class Inner: RefComparer<T>
		{
		}

		#region IRefComparer implementation
		public int Compare (ref T a, ref T b)
		{
			return a.CompareTo(ref b);
		}
		#endregion
	}

	#endregion

	public interface IComparand<T>
	{
		int CompareWith(ref T left);
	}
	 
	public struct Comparand<T> : IComparand<T> where T : IComparable<T>
	{
		readonly T _val;
		public Comparand(T val)
		{
			_val = val;
		}

		public int CompareWith(ref T left)
		{
			return left.CompareTo(_val);
		}

	}

	static public class Salgo
	{
		static int flexibleIndex<T> (IList<T> lst, int idx)
		{
			var cnt = lst.Count;
			return idx < 0 ? cnt - idx : idx > cnt ? cnt : idx;
		}

		#region LowerBound
		static public int LowerBound<T> (this IList<T> lst, int begin, int end, T val, Comparison<T> comp)
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;					
				if (comp(lst[mid], val) < 0)
				{
					begin = mid + 1;
				}
				else
				{ 
					end = mid;
				}
			}
			return begin;			
		}

		static public int LowerBound<T> (this IList<T> lst, T val, Comparison<T> comp)
		{
			return LowerBound(lst, 0, lst.Count, val, comp);
		}

		static public int LowerBound<T> (this IList<T> lst, int begin, int end, T val)
		{
			return LowerBound(lst, begin, end, val, Comparer<T>.Default.Compare);
		}

		static public int LowerBound<T> (this IList<T> lst, T val)
		{
			return LowerBound(lst, 0, lst.Count, val, Comparer<T>.Default.Compare); 
		}
		#endregion

		#region LowerBound for Array using Reference Comparison
		static public int LowerBound<T> (this T[] arr, int begin, int end, ref T val, RefComparison<T> comp)
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;
				if (comp(ref arr[mid], ref val) < 0)
				{
					begin = mid + 1;					
				}
				else
				{
					end = mid;
				}
			}
			return begin;			
		}
		static public int LowerBound<T,C> (this T[] arr, int begin, int end, C comp) where C : IComparand<T>
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;
				if (comp.CompareWith(ref arr[mid]) < 0)
				{
					begin = mid + 1;					
				}
				else
				{
					end = mid;
				}
			}
			return begin;			
		}
		static public int LowerBound<T> (this T[] arr, int begin, int end, ref T val) where T: IRefComparable<T>
		{
			return arr.LowerBound(begin, end, ref val, RefComparer<T>.Comparison);
		}

		static public int LowerBound<T> (this T[] arr, ref T val, RefComparison<T> comp)
		{
			return arr.LowerBound(0, arr.Length, ref val, comp);
		}

		static public int LowerBound<T> (this T[] arr, ref T val)where T: IRefComparable<T>
		{
			return arr.LowerBound(0, arr.Length, ref val, RefComparer<T>.Comparison);
		}

		#endregion

		
		#region UpperBound
		static public int UpperBound<T> (this IList<T> lst, int begin, int end, T val, Comparison<T> comp)
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;
				if (comp(lst[mid], val) <= 0)
				{
					begin = mid + 1;
				}
				else
				{
					end = mid;
				}
			}
			return begin;
		}

		static public int UpperBound<T,C> (this T[] arr, int begin, int end, C comp) where C : IComparand<T>
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;
				if (comp.CompareWith(ref arr[mid]) <= 0)
				{
					begin = mid + 1;
				}
				else
				{
					end = mid;
				}
			}
			return begin;
		}

		static public int UpperBound<T> (this IList<T> lst, int begin, int end, T val)
		{
			return lst.UpperBound(begin, end, val, Comparer<T>.Default.Compare);
		}

		static public int UpperBound<T> (this IList<T> lst, T val, Comparison<T> comp)
		{
			return lst.UpperBound(0, lst.Count, val, comp);
		}

		static public int UpperBound<T> (this IList<T> lst, T val)
		{
			return lst.UpperBound(0, lst.Count, val, Comparer<T>.Default.Compare);
		}

		static public int UpperBound<T> (this T[] arr, int begin, int end, ref T val, RefComparison<T> comp)
		{
			while (begin < end)
			{
				int mid = (begin + end) / 2;
				if (comp(ref arr[mid], ref val) <= 0)
				{
					begin = mid + 1;
				}
				else
				{
					end = mid;
				}
			}
			return begin;
		}

		static public int UpperBound<T> (this T[] arr, int begin, int end, ref T val)where T: IRefComparable<T>
		{
			return arr.UpperBound(begin, end, ref val, RefComparer<T>.Default.Compare);
		}

		static public int UpperBound<T> (this T[] arr, ref T val, RefComparison<T> comp)
		{
			return arr.UpperBound(0, arr.Length, ref val, comp);
		}

		static public int UpperBound<T> (this T[] arr, ref T val) where T: IRefComparable<T>
		{
			return arr.UpperBound(0, arr.Length, ref val, RefComparer<T>.Default.Compare);
		}

		#endregion

		#region EqualRange
		static public bool EqualRange<T> (
			this IList<T> lst, ref int begin, ref int end, T val, Comparison<T> comp)
		{

			int lo = lst.LowerBound(begin, end, val, comp);

			if (lo == end)
			{
				begin = end;
			}
			else if (lo == begin && comp(lst[begin], val) > 0)
			{//TODO
				end = begin;
			}
			else
			{
				end = lst.UpperBound(begin = lo, end, val, comp);
			}
			return begin != end;
		}

		static public bool EqualRange<T> (this IList<T> lst, ref int begin, ref int end, T val)
		{
			return lst.EqualRange(ref begin, ref end, val, Comparer<T>.Default.Compare);
		}

		static public ListSlice<T> EqualSlice<T> (this IList<T> lst, T val, Comparison<T> comp)
		{
			int begin = 0, end = lst.Count;
			return lst.EqualRange(ref begin, ref end, val, comp) ? 
				new ListSlice<T>(lst, begin, end) :
					ListSlice<T>.Empty;
		}

		static public ListSlice<T> EqualSlice<T> (this IList<T> lst, T val)
		{
			return lst.EqualSlice(val, Comparer<T>.Default.Compare);
		}

		static public bool EqualRange<T> (
			this T[] arr, ref int begin, ref int end, ref T val, RefComparison<T> comp)
		{
			int lo = arr.LowerBound(begin, end, ref val, comp);

			if (lo == end)
			{
				begin = end;
			}
			else if (lo == begin && comp(ref arr[begin], ref val) > 0)
			{//TODO
				end = begin;
			}
			else
			{
				end = arr.UpperBound(begin = lo, end, ref val, comp);
			}
			return begin != end;
		}

		static public bool EqualRange<T,C> (
			this T[] arr, ref int begin, ref int end, C comp) where C : IComparand<T> 
		{
			int lo = arr.LowerBound(begin, end, comp);

			if (lo == end)
			{
				begin = end;
			}
			else if (lo == begin && comp.CompareWith(ref arr[begin]) > 0)
			{
				end = begin;
			}
			else
			{
				end = arr.UpperBound(begin = lo, end, comp);
			}
			return begin != end;
		}

		static public bool EqualRange<T> (this T[] arr, ref int begin, ref int end, ref T val) where T:IRefComparable<T>
		{
			return arr.EqualRange(ref begin, ref end, ref val, RefComparer<T>.Default.Compare);
		}

		static public ArraySlice<T> EqualSlice<T> (this T[] arr, ref T val, RefComparison<T> comp)
		{
			int b = 0, end = arr.Length;
			return arr.EqualRange(ref b, ref end, ref val, comp) ?
				new ArraySlice<T>(arr, b, end) :
					ArraySlice<T>.Empty;
		}

		static public ArraySlice<T> EqualSlice<T> (this T[] arr, ref T val) where T : IRefComparable<T>
		{
			return arr.EqualSlice(ref val, RefComparer<T>.Default.Compare);
		}
		#endregion

		static public int BinarySearch<T> (this T[] arr, int begin, int len, T val, Comparison<T> comp)
		{
			int end = begin + len;
				
			while (begin < end)
			{
				int mid = (begin + end) / 2,
				rel = comp(arr[mid], val);
				if (rel < 0)
				{
					begin = mid + 1;
				}
				else if (rel > 0)
				{
					end = mid;
				}
				else
				{
					return mid;
				}
			}
			return ~begin;
		}

		static public int BinarySearch<T,C> (this T[] arr, int begin, int len,C comp) where C : IComparand<T>
		{
			int end = begin + len;
				
			while (begin < end)
			{
				int mid = (begin + end) / 2,
				rel = comp.CompareWith(ref arr[mid]);
				if (rel < 0)
				{
					begin = mid + 1;
				}
				else if (rel > 0)
				{
					end = mid;
				}
				else
				{
					return mid;
				}
			}
			return ~begin;
		}


		#region other stuff

		static public IEnumerable<KeyValuePair<V,K>> Flip<K,V>(this IEnumerable<KeyValuePair<K,V>> kvs)
		{
			return kvs.Select(kv=>new KeyValuePair<V,K>(kv.Value,kv.Key));
		}

		static public void Each<T> (this IEnumerable<T> src, Action<T> act)
		{
			foreach (T el in src)
			{
				act(el);
			}
		}

		static public ArraySlice<T> Slice<T> (this T[] arr, int begin, int end)
		{
			return new ArraySlice<T>(arr, begin, end);
		}

		static public ArraySlice<T> Slice<T> (this T[] arr, int begin)
		{
			return new ArraySlice<T>(arr, begin, arr.Length);
		}

		static public ListSlice<T> Slice<T> (this IList<T> lst, int begin, int end)
		{
			return new ListSlice<T>(lst, begin, end);
		}

		static public ListSlice<T> Slice<T> (this IList<T> lst, int begin)
		{
			return new ListSlice<T>(lst, begin, lst.Count);
		}

		static public AdaptorSlice<T,U> Cast<T,U> (this ISlice<T> slice, Func<T,U> adaptor)
		{
			return new AdaptorSlice<T, U>(slice, adaptor);
		}
		#endregion


	}
}

