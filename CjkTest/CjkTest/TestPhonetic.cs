using System;
using NUnit.Framework;
using Cjk.Phonetic;
using Salgo;
using System.Linq;
namespace CjkTest
{
	[TestFixture()]
	public class TestPhonetic
	{
		[Test()]
		public void TestER ()
		{
			var range = ESyllable.GetAllByChar('䣧');
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
		[Test]
		public void TestRimes()
		{
			var rimes = ESection.GetAllByChar('樂');
			CollectionAssert.AreEquivalent(new char[]{'效','鐸','覺'},rimes.Select(er=>er.Title));
			CollectionAssert.AreEquivalent(new char[]{'效','覺','藥'},
				Rime106.GetAllByChar('樂').Select(r=>r.Title)
			);

			var rime = Rime106.GetByTitle('咸');
			Assert.True(rime.Contains('鹹'));
			Assert.AreEqual('咸',rime.Title);
			var league = rime.League;
			Assert.AreEqual('豏',league.Rising.Value.Title);
			Assert.AreEqual('陷',league.Going.Value.Title);
			Assert.AreEqual('洽',league.Entering.Value.Title);
			rime = Rime106.GetByTitle('有');
			Assert.False(rime.Peer(Tone.Entering).HasValue);
			Assert.AreEqual('宥',rime.Peer(Tone.Going).Value.Title);
			var r19s = Rime19.GetAllByChar('蛙');
			CollectionAssert.AreEquivalent(new int[]{5,10},r19s.Select(r=>r.Ordinal));
			Assert.True(r19s.All(r=>r.Volume == 1));
			r19s = Rime19.GetAllByChar('會');
			Assert.AreEqual(1,r19s.Count());
			Assert.AreEqual(3,r19s.First().Ordinal);
			Assert.AreEqual(3,r19s.First().Volume);
			Assert.True(r19s.First().Contains('會'));
		}
	}
}

