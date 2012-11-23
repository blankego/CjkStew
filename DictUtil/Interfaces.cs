using System;
using System.Collections.Generic;
namespace DictUtil
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

		byte[] GetBytes (long offset, int length);
	}

	public interface IDictAddress
	{
		long Offset { get;}
		int Length { get;}
	}





	public interface IDictIDX
	{
		// use constraint to avoid boxing of value types
		bool GetAddress (int ordinal, out long offset, out int length);

		bool GetIndexRange(string headword, out int begin, out int end);
	}
}

