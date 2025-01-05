namespace PuzzleManager.Domain.Entities
{
	/// <summary>
	/// Represents the record of a puzzle being checked out by a puzzle holder.
	/// </summary>
	public class PuzzleCheckout
	{
		/// <summary>
		/// Primary key for the PuzzleCheckout entity.
		/// </summary>
		public int PuzzleCheckoutId { get; set; }

		/// <summary>
		/// Foreign key to the Puzzle being checked out.
		/// </summary>
		public int PuzzleId { get; set; }

		/// <summary>
		/// Foreign key to the holder (family member or friend) checking out the puzzle.
		/// </summary>
		public int PuzzleHolderId { get; set; }


		/// <summary>
		/// The date/time the puzzle was checked out.
		/// </summary>
		public DateTime CheckoutDate { get; set; }

		/// <summary>
		/// The date/time the puzzle was returned.
		/// </summary>
		public DateTime? ReturnDate { get; set; }

		/// <summary>
		/// The total time it took to complete, in hours (or days).
		/// Could also be derived from CheckoutDate and ReturnDate.
		/// </summary>
		public double? TimeToComplete { get; set; }

		/// <summary>
		/// Optional user feedback on difficulty, which might differ from the puzzle's overall difficulty rating.
		/// </summary>
		public double? UserDifficultyRating { get; set; }

		/// <summary>
		/// Navigation property to the puzzle holder who checked out the puzzle.
		/// </summary>
		public PuzzleHolder PuzzleHolder { get; set; } = default!;

		public Puzzle Puzzle { get; set; } = default!;
	}
}
