using System;
using System.Collections.Generic;
using System.Collections;
using Salgo;
using System.Linq;

namespace Cjk
{
	public class ArrayIndex<K,V>:IEnumerable<KeyValuePair<K,V>> where K :IComparable<K>
	{
		struct KeyComparand : IComparand<KeyValuePair<K,V>>
		{
			internal K _key;			

			public int CompareWith (ref KeyValuePair<K, V> left)
			{
				return left.Key.CompareTo(_key);
			}
		}

		KeyValuePair<K,V>[] _arr;

		public int Length{ get; private set; }

		public ArrayIndex (IEnumerable<KeyValuePair<K,V>> iter):this()
		{
			foreach (var item in iter)
				Add(item);
			Prep();
		}

		public ArrayIndex (KeyValuePair<K,V>[] arr)
		{
			Length = arr.Length;
			_arr = arr;
			Prep();
		}

		public ArrayIndex (int capacity = 12)
		{
			Length = 0;
			_arr = new KeyValuePair<K, V>[capacity];
		}

		private void _EnsureCapacity (int size)
		{
			if (size + 1 >= _arr.Length)
			{
				Array.Resize(ref _arr, _arr.Length * 2);

			}
		}

		public void Add (KeyValuePair<K,V> kv)
		{
			_EnsureCapacity(Length);
			_arr[Length++] = kv;
		}

		public void Add (K key, V val)
		{
			Add(new KeyValuePair<K, V>(key, val));			
		}

		public void ShrinkToFit ()
		{
			Array.Resize(ref _arr, Length);

		}

		public ArrayIndex<K,V> Prep ()
		{
			ShrinkToFit();
			Array.Sort(_arr, (a,b) => a.Key.CompareTo(b.Key));
			return this;
		}
		public int BinarySearch(K key)
		{
			return _arr.BinarySearch(0, Length, new KeyComparand{_key = key});
		}
		public bool EqualRange (K key, out int begin, out int end)
		{
			begin = 0;
			end = Length;
			return _arr.EqualRange(ref begin, ref end, new KeyComparand{_key = key});
		}

		public IEnumerable<V> EqualRange(K key)
		{
			int begin,end;
			return EqualRange(key, out begin, out end) ? 
				Enumerable.Range(begin,end - begin).Select(i=>_arr[i].Value):
					Enumerable.Empty<V>();
		}

		public V GetValue (int idx)
		{
			return _arr[idx].Value;
		}


		#region IEnumerable implementation
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator ()
		{
			for (int i = 0; i != Length; ++i)
				yield return  _arr[i];
		}
		#endregion


	}
}

