using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Salgo;
using System.Collections.Generic;

namespace DictUtil
{
	public class StarDictInfo
	{
		public readonly string 
			FileName,
			Version, Name, Author = "", EMail = "", Description = "",
			WebSite = "", Date = "";
		public readonly int 
			NumberOfEntries = 0, NumberOfSynonyms = 0, 
			IndexFileSize = 0, PointerSize = 4;
		public readonly char TypeMark;
		public string BaseName { get; private set; }
		public StarDictInfo (string path)
		{
			FileName = path;
			BaseName = Path.Combine(Path.GetFullPath(Path.GetDirectoryName(path)),Path.GetFileNameWithoutExtension(path));
			foreach (var term in File.ReadAllLines(path)
			        .Select(l=>l.Split(new char[]{'='}).Select(it=>it.Trim()).ToArray())
			        .Where(t=>t.Length == 2))
			{
				switch (term[0])
				{
				case "bookname":
					Name = term[1];
					break;
				case "version":
					Version = term[1];
					break;
				case "wordcount":
					NumberOfEntries = int.Parse(term[1]);
					break;
				case "idxfilesize":
					IndexFileSize = int.Parse(term[1]);
					break;
				case "author":
					Author = term[1];
					break;
				case "email":
					EMail = term[1];
					break;
				case "date":
					Date = term[1];
					break;
				case "description":
					Description = term[1];
					break;
				case "sametypesequence":
					TypeMark = term[1][0];
					break;
				case "idxoffsetbits":
					PointerSize = term[1] == "64" ? 8 : 4;
					break;
				case "synwordcount":
					NumberOfSynonyms = int.Parse(term[1]);
					break;
				}

			}
			if (PointerSize == 0)
				PointerSize = 2;

		}

		public bool IsValid {
			get {
				return /*(Version == "3.0.0" || Version == "2.4.8") && */ Name != null && 
					NumberOfEntries != 0 && IndexFileSize != 0 &&
					(NumberOfSynonyms == 0 || 
					File.Exists(Path.GetFileNameWithoutExtension(FileName) + ".syn"));				
			}
		}
	}
	
}
