using System.Collections.Generic;

namespace PuzzleManager.Domain
{
	/// <summary>
	/// Represents the company or individual who made the puzzle.
	/// </summary>
	public class PuzzleMaker
	{
		/// <summary>
		/// Primary key for the PuzzleMaker entity.
		/// </summary>
		public int PuzzleMakerId { get; set; }

		/// <summary>
		/// Name of the puzzle maker (e.g., brand or manufacturer).
		/// </summary>
		public string Name { get; set; } = default!;

		/// <summary>
		/// An optional field for location or country of the maker, if relevant.
		/// </summary>
		public string? Country { get; set; }

		/// <summary>
		/// Navigation property: the puzzles made by this maker.
		/// </summary>
		public ICollection<Puzzle>? Puzzles { get; set; }
	}
}
