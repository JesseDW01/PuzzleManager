using PuzzleManager.Services.DTOs;

namespace PuzzleManager.Services.Interfaces
{
	/// <summary>
	/// Defines the contract for scraping puzzle data from external sources.
	/// </summary>
	public interface IPuzzleScraper
	{
		/// <summary>
		/// Retrieves detailed information about a specific puzzle using its URL.
		/// </summary>
		/// <param name="puzzleUrl">The full URL of the puzzle page to scrape.</param>
		/// <returns>
		/// A task that represents the asynchronous operation.
		/// The task result contains a <see cref="JVHOnlinePuzzleDto"/> object with puzzle details,
		/// or <c>null</c> if the puzzle could not be found or scraped successfully.
		/// </returns>
		Task<JVHOnlinePuzzleDto?> GetPuzzleByUrlAsync(string puzzleUrl);

		/// <summary>
		/// Searches for puzzles that match the provided search term.
		/// </summary>
		/// <param name="searchTerm">The term to search for within puzzle titles or descriptions.</param>
		/// <returns>
		/// A task that represents the asynchronous operation.
		/// The task result contains a list of <see cref="JVHOnlinePuzzleDto"/> objects that match the search criteria.
		/// </returns>
		Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm);
	}
}
