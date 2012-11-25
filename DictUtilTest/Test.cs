using System;
using NUnit.Framework;
using DictUtil;
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
			var gz = GZip.OpenRead("lorem_ipsum.txt.gz");
			Assert.AreEqual(2838, gz.OriginalSize);
			Assert.AreEqual(1276, gz.CompressedFileSize);
			Assert.AreEqual("56.24 %", gz.Ratio);
			Assert.AreEqual(CompressionLevel.Fastest, gz.CompressionLevel);
			Assert.AreEqual(26, gz.HeaderSize);
			Assert.False(gz.HasHeaderCRC);
			Assert.True(gz.HasName);
			Assert.AreEqual("lorem_ipsum.txt", gz.Name);
			CollectionAssert.AreEqual(content, gz.ReadAllBytes());
			string path = "new_lorem.gz";
			using(gz = GZip.Create(path,true))
			{
				gz.Name = "new_lorem";
				gz.Comment = "test";
				gz.Write(content,0,content.Length - 10);
				gz.Write(content,content.Length - 10, 10);
			}

			using(var gz1 = GZip.OpenRead(path))
			{
				byte[] gzContent = gz1.ReadAllBytes();
				Assert.AreEqual("new_lorem",gz1.Name);
				Assert.AreEqual("test",gz.Comment);
				Assert.AreEqual(content.Length,gz1.OriginalSize);
				Assert.AreEqual(Crc32.UpdateCrc(content,0,content.Length),gz1.Crc);
				CollectionAssert.AreEqual(content,gzContent);
			}
			using(var fs = File.OpenRead("lorem_ipsum.txt"))
			using(gz = GZip.Create("foo.gz",true))
			{
				gz.Compress(fs);
			}
			using(gz = GZip.OpenRead("foo.gz"))
			{
				CollectionAssert.AreEqual(content, gz.ReadAllBytes());
			}
		}

		[Test]
		public void TestDzip ()
		{
			using(var dz = DictZip.OpenRead("sm_entry.csv.dz"))
			{
				Assert.AreEqual(81, dz.ExtraField.ChunkCount);
				Assert.AreEqual(58315, dz.ExtraField.ChunkSize);
//				byte[] content = File.ReadAllBytes("sm_entry.csv");
//				CollectionAssert.AreEqual(content, dz.ReadAllBytes());
				string s = "540,500,文一  重一,,,\n";
				Assert.AreEqual(s, dz.GetEntry((int)dz.OriginalSize - 26, 26));
				using(var f = File.OpenRead("sm_entry.csv"))
				using(var dz2 = DictZip.Create("foo.dz",true))
				{
					dz2.Name = "dz2";
					dz2.Comment= "no comment";
					dz2.Compress(f);
				}
				using(var dz2 = DictZip.OpenRead("foo.dz"))
				{
					Assert.AreEqual("dz2",dz2.Name);
					Assert.AreEqual("no comment",dz2.Comment);
					Assert.AreEqual(81,dz2.ExtraField.ChunkCount);
					Assert.AreEqual(s,dz2.GetEntry((int)dz.OriginalSize - 26,26));
				}
			}
		}

		[Test]
		public void TestEndiannessConversion ()
		{
			byte[] arr = {0x01,0x23,0x45,0x67,0x89,0xab,0xcd,0xef};
			Assert.AreEqual(0x0123456789abcdefUL, arr.GetLongBigEndian());
			Assert.AreEqual(0xefcdab8967452301UL, arr.GetLongLittleEndian());
			Assert.AreEqual(0x89abcdefU, arr.GetIntBigEndian(4));
			Assert.AreEqual(0x8967,arr.GetIntLittleEndian(3,2));
		}

		[Test]
		public void TestStarDictInfo()
		{
			var info = new StarDictInfo(Path.GetFullPath( @"data/jargon.ifo"));
			Assert.AreEqual("jargon.ifo",Path.GetFileName(info.FileName));
			Assert.AreEqual(4,info.PointerSize);
			Assert.AreEqual("jargon",Path.GetFileName(info.BaseName));
			Assert.AreEqual('m',info.TypeMark);
			Console.WriteLine(info.BaseName);
		}

		[Test]
		public void TestStarDictIdx()
		{
			var info = new StarDictInfo("data/jargon.ifo");
			var idx = new StarDictIdx(info);

		}

		[Test]
		public void TestCrc32()
		{
			using(var fs = File.OpenRead("lorem_ipsum.txt"))
			{
				byte[] bs = File.ReadAllBytes("lorem_ipsum.txt");
				Assert.AreEqual(0x6029fcc0U,Crc32.Compute(bs));
				fs.Position = 0;
				Assert.AreEqual(0x6029fcc0U,Crc32.Compute(fs));
				Assert.AreEqual(fs.Position, fs.Length);
			}
		}
	}
}

