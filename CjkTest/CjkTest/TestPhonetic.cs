using System;
using NUnit.Framework;
using Cjk.Phonetic;
using Salgo;
namespace CjkTest
{
	[TestFixture()]
	public class TestPhonetic
	{
		[Test()]
		public void TestER ()
		{
			var range = ESyllable.GetAll('䣧');
			Assert.AreEqual(1,range.Count);
			var syl = range[0];
			Assert.AreEqual("弋翊翌廙黓翼𩙺𦏵䴬隿𧃟㚤潩杙芅㔴𧾰𨙒𥡪蛡𢦭𢖺瀷𢎀𠥦熼釴𤼌𧑌𦔜䄩𢓀䘝䣧",syl.HeadWos.ToString());
			Assert.AreEqual('以', syl.Initial.Name);
			Assert.AreEqual("蒸",syl.Final.Name);
			Assert.True(syl.Final.Clicked);
			Assert.AreEqual('職', syl.Section.Title);

			Assert.True(ER.IsLevel('東'));
			Assert.True(ER.IsOblique('共'));
			Assert.True(ER.IsLevel('共'));
		}
	}
}

