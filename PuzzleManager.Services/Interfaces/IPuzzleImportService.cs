using PuzzleManager.Services.DTOs;
using PuzzleManager.Domain;
using Microsoft.AspNetCore.Identity;

namespace PuzzleManager.Services.Interfaces
{
	/// <summary>
	/// Interface for importing puzzles from external sources.
	/// </summary>
	public interface IPuzzleImportService
	{
		/// <summary>
		/// Searches for puzzles based on a search term.
		/// </summary>
		/// <param name="searchTerm">The term to search for.</param>
		/// <returns>A list of matching puzzle DTOs.</returns>
		Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm);

		/// <summary>
		/// Imports a puzzle by its URL and associates it with a holder.
		/// </summary>
		/// <param name="puzzleUrl">The URL of the puzzle to import.</param>
		/// <param name="holderId">The ID of the puzzle holder.</param>
		/// <returns>The imported Puzzle entity.</returns>
		Task<Puzzle?> ImportPuzzleByUrlAsync(string puzzleUrl, IdentityUser user);
	}
}
