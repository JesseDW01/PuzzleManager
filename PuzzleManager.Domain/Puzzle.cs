namespace PuzzleManager.Domain
{
	/// <summary>
	/// Represents a jigsaw puzzle in the puzzle management system.
	/// </summary>
	public class Puzzle
	{
		/// <summary>
		/// Primary key for the Puzzle entity.
		/// </summary>
		public int PuzzleId { get; set; }

		/// <summary>
		/// The name or title of the puzzle.
		/// </summary>
		public string Name { get; set; } = default!;

		/// <summary>
		/// The total number of pieces for this puzzle.
		/// </summary>
		public int PieceCount { get; set; }

		/// <summary>
		/// An overall rating of difficulty (e.g. from 1-5).
		/// </summary>
		public double DifficultyRating { get; set; }

		/// <summary>
		/// Foreign key reference to the puzzle maker.
		/// </summary>
		public int PuzzleMakerId { get; set; }

		/// <summary>
		/// Navigation property to the puzzle maker.
		/// </summary>
		public PuzzleMaker Maker { get; set; } = default!;

		/// <summary>
		/// Navigation property: all the checkouts that have happened for this puzzle.
		/// </summary>
		public ICollection<PuzzleCheckout> PuzzleCheckouts { get; set; } = [];


	}
}
