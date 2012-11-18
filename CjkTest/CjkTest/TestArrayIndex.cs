using System;
using NUnit.Framework;
using Cjk;
using Salgo;
namespace CjkTest
{
	[TestFixture]
	public class TestArrayIndex
	{
		[Test]
		public void TestAI()
		{
			var ai = new ArrayIndex<CodePoint,int>{{'這',1},{'在',2},{'謟',5},{'謟',0},{'A',3}};
			ai.Prep();
			Assert.AreEqual(5,ai.Length);
			int b = 0, e = ai.Length;
			ai.EqualRange('謟',out b,out e);
			Assert.AreNotEqual(b,e);
			Assert.True(e>b);
		}
	}
}

