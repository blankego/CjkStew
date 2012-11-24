using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Salgo;
using System.Linq;

namespace salgo_test
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		unsafe public void TestCodePoint ()
		{
			var s1 = "𢐈";
			CodePoint cp = s1;
			Assert.AreEqual (3, sizeof(CodePoint));
			Assert.IsFalse (cp.InBmp);
			Assert.AreEqual (s1, cp.ToString ());
			var s2 = "要";
			var s3 = "å";
			cp = s2;
			Assert.AreEqual (s2, cp.ToString ());
			cp = s3;
			Assert.AreEqual (s3, cp.ToString ());
			CodePoint[] cpArr = {"的","a","b","𢐈","的"};

			fixed(CodePoint* p = cpArr) {
				Assert.AreEqual (6, (uint)(p + 2) - (uint)p);
				Assert.AreEqual ("的", p->ToString ());
			}

		}

		[Test]
		public void TestUString ()
		{
			var s = "ab要蘋𢐈å";
			UString us = new UString (s);
			Assert.AreEqual (7, us.StrLength);
			Assert.AreEqual (6, us.Length);
			Assert.AreEqual (s, us.ToString ());
			var lastCp = us.Last ();
			Assert.AreEqual ("å".ToCodePoint (), lastCp);
			Assert.AreEqual ("å".ToCodePoint (), us [5]);
			var s2 = new string ('正', 0x100000);
			//Global heap allocation
			Assert.AreEqual (s2, s2.ToUString ().ToString ());

		}

		struct Foo :IRefComparable<Foo>
		{
			public readonly long A, B;

			public Foo (int x, int y)
			{
				A = x;
				B = y;
			}
			#region IRefComparable implementation
			public int CompareTo (ref Foo other)
			{
				return (int)(A - other.A);
			}
			#endregion

		}

		[Test]
		public void TestAlgo ()
		{
			int[] arr = {1,3,3,4,6,6,6,6,7,9,12,13,13,13,13,15,17,19,20,20};

			Assert.AreEqual (4, arr.LowerBound (6));
			Assert.AreEqual (8, arr.UpperBound (6));
			var slice = arr.EqualSlice (6);
			Assert.AreEqual (4, slice.Count);
			Assert.AreEqual (4, slice.Begin);
			Assert.AreEqual (8, slice.End);

			Foo[] fArr = {
				new Foo (1, 0),
				new Foo (1, 1),
				new Foo (2, 0),
				new Foo (5, 0),
				new Foo (5, 6),
				new Foo (5, 8),
				new Foo (7, 0),
				new Foo (10, 0),
			};
			Foo f8 = new Foo (8, 0);
			Foo f5 = new Foo (5, 11);
			Assert.AreEqual (7, fArr.LowerBound (ref f8));
			Assert.AreEqual (7, fArr.UpperBound (ref f8));
			var fSlice = fArr.EqualSlice (ref f5);
			Assert.AreEqual (3, fSlice.Count);
			Assert.AreEqual (3, fSlice.Begin);
			Assert.AreEqual (6, fSlice.End);
			var emp = fArr.EqualSlice (ref f8);
			Assert.AreSame (ArraySlice<Foo>.Empty, emp);
			Assert.IsTrue (emp.Count == 0);


			CollectionAssert.AreEqual (fArr.Skip (3).Take (3), fSlice);
			var sSlice = fSlice.Cast (a => a.B.ToString ());
			Assert.AreEqual (3, sSlice.Count);
			CollectionAssert.AreEqual (new string[]{"0","6","8"}, sSlice);
			int idx = arr.BinarySearch(0,arr.Length,20,(a,b)=>a-b);
			Console.WriteLine(idx);
			Assert.True(Array.IndexOf(arr,20)<= idx && idx < (arr.Length));
			idx = arr.BinarySearch(0, arr.Length,3,(a,b)=>a-b);
			var es = arr.EqualSlice(3);
			Assert.True(es.Begin <= idx && idx < es.End);
		}
	}
}

