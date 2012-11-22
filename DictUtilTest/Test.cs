using System;
using NUnit.Framework;
using DictTypes;
using System.IO;

namespace DictTypesTest
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestGzip ()
		{
			byte[] content = File.ReadAllBytes("lorem_ipsum.txt");
			var gz = new GZip("lorem_ipsum.txt.gz");
			Assert.AreEqual(2838,gz.OriginalSize);
			Assert.AreEqual(1276,gz.CompressedFileSize);
			Assert.AreEqual("56.24 %",gz.Ratio);
			Assert.AreEqual(CompressionLevel.Fastest,gz.CompressionLevel);
			Assert.AreEqual(26, gz.HeaderSize);
			Assert.False(gz.HasHeaderCRC);
			Assert.True(gz.HasName);
			Assert.AreEqual("lorem_ipsum.txt",gz.Name);
			CollectionAssert.AreEqual(content, gz.ReadAllBytes());
		}

		[Test]
		public void TestDzip()
		{
			var dz = new DictZip("sm_entry.csv.dz");
			Assert.AreEqual(81, dz.ChunkCount);
			Assert.AreEqual(58315,dz.ChunkLength);
//			byte[] content = File.ReadAllBytes("sm_entry.csv");
//			CollectionAssert.AreEqual(content, dz.ReadAllBytes());
			string s = "540,500,文一  重一,,,\n";
			Assert.AreEqual(s, dz.GetEntry((int)dz.OriginalSize - 26, 26));
			Assert.AreEqual(s, dz.GetEntry((int)dz.OriginalSize - 26, 26));
		}
	}
}

