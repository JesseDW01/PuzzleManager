using Microsoft.Identity.Client;

namespace PuzzleManager.Services.DTOs
{
	/// <summary>
	/// Represents puzzle info scraped or retrieved from janvanhaasteren.nl
	/// </summary>
	public class JVHOnlinePuzzleDto
	{
		public string Title { get; set; } = default!;
		public string FullTitle { get; set; } = default!;
		public int PieceCount { get; set; }
		public int ArticleNumber { get; set; }
		public string? Artist { get; set; }
		public int? Year { get; set; }
		public string? ImageUrl { get; set; }
		public string? ProductUrl { get; set; }

		public readonly string Maker = "Jan van Haasteren";

		// ... other fields you might parse (e.g., description, release date)
	}
}
