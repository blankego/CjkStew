using System;

namespace DictTypes
{
	public interface IDictDB
	{
		/// <summary>
		/// Retrieves a entry from dictionary the database (plain txt or dictzip)
		/// </summary>
		/// <returns>
		/// The entry string, encoding conversion should be done by the implementation
		/// </returns>
		/// <param name='offset'>
		/// Offset. Currently rarely a dictionary would be bigger than 4GB, 
		/// yet make it long anyway for future extension.
		/// </param>
		/// <param name='length'>
		/// Length.
		/// </param>
		string GetEntry (long offset, int length);
	}

	/// <summary>
	/// Unifies various address pairs in this from. No matter what magnitude of number you choose
	/// to use for your address pair, as long as it complies with these two properties' signature 
	/// it will be suitable to the whole API
	/// </summary>
	public interface IDictAddress
	{
		long Offset{get;set;}
		int Length{get;set;}
	}

	public interface IDictIDX
	{
		// use constraint to avoid boxing of value types
		bool GetIndex<A>(int ordinal, ref A address) where A : IDictAddress;
		bool GetIndex<A>(string headword, ref A address) where A : IDictAddress;
	}
}

