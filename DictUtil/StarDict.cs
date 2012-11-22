using System;

namespace DictTypes
{
	public class StarDict
	{
		public string Name { get; protected set;}
		public string NumberOfEntries { get; protected set;}
		public int IndexFileSize { get; protected set;}
		public int PointerSize{ get; protected set;}
		public string Author { get; protected set;}
		public string EMail { get; protected set;}
		public string Description { get; protected set;}
		public string Website {get; protected set;}
		public string Date { get; protected set;}
		protected string SameTypeSequence { get; set;}
		public StarDict ()
		{
		}
		void ParseInfo()
		{

		}
		void LoadIndex()
		{

		}

		string Search(string headword)
		{
			throw new NotImplementedException();
		}

		void GetAddress(string hw, out int offset, out int length)
		{

		}
	}
}

