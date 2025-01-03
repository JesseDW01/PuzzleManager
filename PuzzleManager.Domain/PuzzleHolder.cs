using System.Collections.Generic;

namespace PuzzleManager.Domain
{
	/// <summary>
	/// Represents a family member or friend who can check puzzles in and out.
	/// </summary>
	public class PuzzleHolder
	{
		/// <summary>
		/// Primary key for the PuzzleHolder entity.
		/// </summary>
		public int PuzzleHolderId { get; set; }

		/// <summary>
		/// The display name of the holder (e.g. "Alice", "Bob").
		/// </summary>
		public string Name { get; set; } = default!;

		/// <summary>
		/// Optional contact email or phone, if relevant.
		/// </summary>
		public string? Contact { get; set; }

		/// <summary>
		/// Navigation property: all checkouts associated with this holder.
		/// </summary>
		public List<PuzzleCheckout>? PuzzleCheckouts { get; set; }
	}
}
