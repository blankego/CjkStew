using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO.Compression;
using System.IO;
using System.Linq;
using Salgo;
using System.Collections.Generic;
using System.Text;

namespace Cjk.Phonetic
{
	[AttributeUsage(AttributeTargets.Struct)]
	class EntTableAttribute :Attribute
	{
		public readonly string Name;

		public EntTableAttribute (string name)
		{
			Name = name;
		}
	}

	delegate void FieldsSetter<T> (ref T st,int idx,string[] rec);

	struct TableImporter<T>
	{
		static char[] _sep = {','};

		internal static T[] Import (FieldsSetter<T> setter,string tableName = null)
		{
			string name = tableName ?? ((EntTableAttribute)typeof(T).GetCustomAttributes(false).First(o => o is EntTableAttribute)).Name;

			Assembly asm = Assembly.GetExecutingAssembly();
			using (Stream s = asm.GetManifestResourceStream(typeof(ErData.ErDataTag),name))
			using (Stream ds = new DeflateStream(s,CompressionMode.Decompress))
			using (TextReader reader = new StreamReader(ds))
			{
				int len = int.Parse(reader.ReadLine()),
				idx = 0;
				T[] arr = new T[len];
				for (string line; (line = reader.ReadLine()) != null; ++idx)
				{
					string[] rec = line.Split(_sep);
					setter(ref arr[idx], idx, rec);
				}
				return arr;
			}
		}

	}

	public struct Meld
	{
		public readonly CodePoint Inital, Final;

		public Meld (string s)
		{
			if (s.Length < 2)
			{
				Inital = Final = default(CodePoint);
			}
			else
			{
				Inital = s.ToCodePoint();
				Final = s.ToCodePoint(Inital.InBmp ? 1 : 2);
			}
		}
		public override string ToString ()
		{
			return Inital == default(CodePoint) ? "[]" : string.Format("[{0},{1}]",Inital,Final);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct _Recons
	{
		internal string Dienq, Phuan, Karlgren, Hvang, Lyx, Zjew, PulleyBlank;

		static internal void SetAll (ref _Recons st, ISlice<string> rec)
		{
			st.Dienq = rec[0];
			st.Phuan = rec[1];
			st.Karlgren = rec[2];
			st.Hvang = rec[3];
			st.Lyx = rec[4];
			st.Zjew = rec[5];
			st.PulleyBlank = rec[6];
		}

		public string Get (Reconstruction recons)
		{
			string res = null;
			switch (recons)
			{
			case Reconstruction.Dienq:
				res = Dienq;
				break;
			case Reconstruction.Phuan:
				res = Phuan;
				break;
			case Reconstruction.Karlgren:
				res = Karlgren;
				break;
			case Reconstruction.Hvang:
				res = Hvang;
				break;
			case Reconstruction.Lyx:
				res = Lyx;
				break;
			case Reconstruction.Zjew:
				res = Zjew;
				break;
			default:
				res = PulleyBlank;
				break;
			}
			return res;
		}
	}

	static class Tables
	{

	}
}

