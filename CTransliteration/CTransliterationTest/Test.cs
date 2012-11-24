using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Cjk.Phonetic;
using Cjk.Phonetic.Mandarin;
using SAlgo;
using System.Runtime.InteropServices;
namespace CPhoneticTest
{
	[TestFixture()]
	public class Test
	{
		List<string> sylStrings = new List<string>();
		List<IList<string>> bpmfGworo = new List<IList<string>>();
		[SetUp]
		public void Init()
		{
			using(var reader = new StreamReader("all_syllables.txt"))
			{
				while(reader.Peek() >= 0)
				{
					sylStrings.Add(reader.ReadLine());
				}

			}
			using(var rdr = new StreamReader("bpmf_gworo.txt"))
			{
				while(rdr.Peek() >= 0)
				{
					bpmfGworo.Add(rdr.ReadLine().Split(new char[]{' '}));
				}
			}

		}
		[Test]
		public void TestTokenDict()
		{
			var td = new TokenDict<string>{
				{"b","b"},{"p","p"},{"m","m"},{"f","f"},{"v","v"},
				{"d","d"},{"t","t"},{"n","n"},{"l","l"},
				{"g","g"},{"k","k"},{"ng","ng"},{"h","h"},
				{"j","g"},{"q","k"},{"gn","gn"},{"x","h"},
				{"zh","j"},{"ch","ch"},{"sh","sh"},{"r","r"},
				{"z","tz"},{"c","ts"},{"s","s"},
				
			};
			var td2 = new TokenDict<string>{{"","empty"},{"●","symbol"}};
			string s = "gang",v;
			int p = 0;
			Assert.IsTrue(td2.TryMatchStart(s,ref p, out v));
			Assert.AreEqual("empty",v);
			Assert.AreEqual(0,p);

			s = "b";
			Assert.IsTrue(td.TryMatchStart(s,ref p, out v));
			Assert.AreEqual("b",v);
			Assert.AreEqual(1,p);
			s = "012";
			Assert.IsFalse(td.TryMatchStart(s,ref p,out v));
			Assert.AreEqual(1,p);
			p = 0;
			s = "gnzc";
			Assert.IsTrue(td.TryMatchStart(s,ref p, out v));
			Assert.AreEqual("gn",v);
			Assert.AreEqual(2,p);
			Assert.IsTrue(td.TryMatchStart(s,ref p, out v));
			Assert.AreEqual("tz",v);
			Assert.AreEqual(3,p);
			Assert.IsTrue(td.TryMatchStart(s,ref p, out v));
			Assert.AreEqual("ts",v);
			Assert.AreEqual(4,p);
			Assert.IsFalse(td.TryMatchStart(s,ref p, out v));
			Assert.AreEqual(4,p);

		}

		[Test]
		public void TestAlgo()
		{
			int[] arr = {2,4,5,5,7,8};
			int len = arr.Length;
			var comp = Comparer<int>.Default;
			int l,u;
			l = arr.LowerBound(0,len,0,comp);
			Assert.AreEqual(0,l);

			l = arr.LowerBound(0,len,10,comp);
			Assert.AreEqual(len,l);

			l = arr.LowerBound(0,len,5,comp);
			Assert.AreEqual(2,l);

			u = arr.UpperBound(0,len,5,comp);
			Assert.AreEqual(4,u);

			IntRange ir = arr.EqualRange(0,len,5,comp);
			Assert.AreEqual(2.__(4),ir);

			ir = arr.EqualRange(0,len,1,comp);
			Assert.AreEqual(0.__(0),ir);

			ir = arr.EqualRange(0,len,9,comp);
			Assert.AreEqual(len.__(len),ir);


		}
		[Test]
		public void TestMSyl()
		{
			MSyl juang4 = new MSyl(INIT.J,MED.U,FIN.ANG,TONE.GOING);

			int size = Marshal.SizeOf( juang4 );
			var init = juang4.Initial;
			var med = juang4.Medial;
			var fin = juang4.Final;
			var tone = juang4.Tone;
			var rime = juang4.Rime;

			Assert.AreEqual(2, size);
			Assert.AreEqual(INIT.J, init);
			Assert.AreEqual(MED.U, med);
			Assert.AreEqual(FIN.ANG, fin);
			Assert.AreEqual(TONE.GOING, tone);
			Assert.AreEqual(RIME.UANG, rime);

			MSyl j2 = new MSyl(INIT.J, MED.U,FIN.ANG, TONE.GOING);
			Assert.IsTrue(juang4 == j2);
			j2.Tone = TONE.RISING;
			Assert.AreEqual(TONE.RISING, j2.Tone);
			j2.Initial = INIT.G;
			Assert.AreEqual(INIT.G, j2.Initial);
			j2.Medial = MED._;
			Assert.AreEqual(MED._, j2.Medial);
			Assert.IsFalse(j2.IsPalatized);
			j2.Final = FIN.ENG;
			Assert.AreEqual(FIN.ENG, j2.Final);
			j2.Medial = MED.U;
			Assert.AreEqual(RIME.ONG, j2.Rime);
			Assert.IsFalse(j2.IsPalatized);
			j2.Medial = MED.I;
			Assert.IsTrue(j2.IsPalatized);
			j2.Medial = MED.IU;
			Assert.IsTrue(j2.IsPalatized);
			Assert.AreEqual(INITGROUP.GGRP,j2.InitialGroup);

			//BOOST_CHECK(j2 < juang4);
			
		}

		[Test]
		public void TestBopomofo()
		{
			foreach(string s in sylStrings)
			{

				int idx = 0;
				MSyl syl = Bopomofo.Inst.MunchSyllable(s,ref idx);
				Assert.AreNotEqual(MSyl.Default, syl);
				string s1 = Bopomofo.Inst.Transcribe(syl);
				Assert.AreEqual(s.Trim(),s1);

			}
			string seq = " ㄊㄧㄢㄒㄧㄚˋ。ㄊㄞˋㄆㄧㄥˊ；";
			IList<AltMSyl> lst = Bopomofo.Inst.Parse(seq);
			Assert.AreEqual(" ",lst[0].Noise);
			Assert.AreEqual("ㄊㄧㄢ",lst[1].MSyl.ToString());
			Assert.AreEqual("ㄒㄧㄚˋ",lst[2].MSyl.ToString());
			Assert.AreEqual("。",lst[3].Noise);
			Assert.AreEqual("ㄊㄞˋ",lst[4].MSyl.ToString());
			Assert.AreEqual("ㄆㄧㄥˊ",lst[5].MSyl.ToString());
			Assert.AreEqual("；",lst[6].Noise);
			Assert.AreEqual(7,lst.Count);
		}

		[Test]
		public void TestPinyin()
		{
			string s0="zhuang4";
			int i = 0, i2 =0;
			MSyl syl = AltPinyin.Inst.MunchSyllable(s0,ref i);
			string bpmf = syl.ToString();			
			Assert.AreEqual(Bopomofo.Inst.MunchSyllable(bpmf,ref i2), syl);

			foreach(string s in sylStrings)
			{
				int idx = 0;
				MSyl ms = Bopomofo.Inst.MunchSyllable(s,ref idx);
				string py = AltPinyin.Inst.Transcribe(ms);
				idx = 0;
				MSyl ms1 = AltPinyin.Inst.MunchSyllable(py,ref idx);
				Assert.AreEqual(ms,ms1);
			}


		}

		[Test]
		public void TestMPinyin()
		{
			foreach(string s in sylStrings)
			{
				int idx = 0;
				MSyl ms = Bopomofo.Inst.MunchSyllable(s,ref idx);
				string mpy = Pinyin.Inst.Transcribe(ms);
				idx = 0;
				MSyl ms1 = Pinyin.Inst.MunchSyllable(mpy,ref idx);
				if(!ms.Equals(ms1)) Console.WriteLine ("{3},{0},{1},{2}",ms,ms1,mpy,s);
				Assert.AreEqual(ms,ms1);
			}
		}

		[Test]
		public void TestGwoRo()
		{
			foreach(string s in sylStrings)
			{
				int idx = 0;
				MSyl ms = Bopomofo.Inst.MunchSyllable(s,ref idx);
				string gr = GwoRo.Inst.Transcribe(ms);
				idx = 0;
				MSyl ms1 = GwoRo.Inst.MunchSyllable(gr,ref idx);
				if(!ms.Equals(ms1)) Console.WriteLine ("{3},{0},{1},{2}",ms,ms1,gr,s);
				Assert.AreEqual(ms,ms1);
			}
			foreach(IList<string> lst in bpmfGworo)
			{
				int idx = 0;
				MSyl syl = Bopomofo.Inst.MunchSyllable(lst[0],ref idx);
				Assert.AreEqual(lst[1],GwoRo.Inst.Transcribe(syl));
				idx = 0;
				Assert.AreEqual(syl,GwoRo.Inst.MunchSyllable(lst[1],ref idx));
				syl.Tone = TONE.MUDDY;
				Assert.AreEqual(lst[2],GwoRo.Inst.Transcribe(syl));
				idx = 0;
				Assert.AreEqual(syl,GwoRo.Inst.MunchSyllable(lst[2],ref idx));
				syl.Tone = TONE.RISING;
				Assert.AreEqual(lst[3],GwoRo.Inst.Transcribe(syl));
				idx = 0;
				Assert.AreEqual(syl,GwoRo.Inst.MunchSyllable(lst[3],ref idx));
				syl.Tone = TONE.GOING;
				Assert.AreEqual(lst[4],GwoRo.Inst.Transcribe(syl));
				idx = 0;
				Assert.AreEqual(syl,GwoRo.Inst.MunchSyllable(lst[4],ref idx));
			}
		}
	}
}

