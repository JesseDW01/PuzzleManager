using System.Net.Http;
using HtmlAgilityPack;
using PuzzleManager.Services.DTOs;
using PuzzleManager.Services.Interfaces;

namespace PuzzleManager.Services.Scrapers
{
	public class JanVanHaasterenScraper : IPuzzleScraper
	{
		private readonly HttpClient _httpClient;

		public JanVanHaasterenScraper(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm)
		{
			// 1. Build a search URL or handle how you want to filter
			// If the site doesn't have a search function, you might have to scrape all and filter in-memory.
			// If there's a real "search" param, you'd do something like:
			// var url = "https://janvanhaasteren.nl/puzzels/?s=" + Uri.EscapeDataString(searchTerm);

			string url = "https://janvanhaasteren.nl/puzzels/";
			HttpResponseMessage response = await _httpClient.GetAsync(url);
			response.EnsureSuccessStatusCode();

			string html = await response.Content.ReadAsStringAsync();

			// 2. Parse HTML, create JVHOnlinePuzzleDto list
			List<JVHOnlinePuzzleDto> result = new List<JVHOnlinePuzzleDto>();
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);

			// Example: select puzzle list items
			HtmlNodeCollection puzzleNodes = doc.DocumentNode.SelectNodes("//div[@class='puzzle-item']");
			if (puzzleNodes == null) return result;

			foreach (HtmlNode? node in puzzleNodes)
			{
				// parse puzzle name, piece count, etc.
				HtmlNode titleNode = node.SelectSingleNode(".//h2");
				string title = titleNode?.InnerText.Trim() ?? "Unknown";

				// filter by searchTerm if needed (in-memory):
				if (!string.IsNullOrEmpty(searchTerm) && !title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
					continue;

				// more scraping...
				JVHOnlinePuzzleDto puzzleDto = new JVHOnlinePuzzleDto
				{
					Title = title,
					PieceCount = 1000, // parse real number if available
					ImageUrl = "",     // parse image if any
					ProductUrl = ""    // parse link if any
				};

				result.Add(puzzleDto);
			}

			return result;
		}

		/// <summary>
		/// Scrapes a single puzzle page by URL and returns its details.
		/// </summary>
		/// <param name="puzzleUrl">URL of the puzzle page</param>
		/// <returns>JVHOnlinePuzzleDto or null if failed</returns>
		public async Task<JVHOnlinePuzzleDto?> GetPuzzleByUrlAsync(string puzzleUrl)
		{
			HttpResponseMessage response = await _httpClient.GetAsync(puzzleUrl);
			if (!response.IsSuccessStatusCode) return null;

			string html = await response.Content.ReadAsStringAsync();
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(html);


			JVHOnlinePuzzleDto puzzleDto = ParsePuzzle(doc, puzzleUrl);


			return puzzleDto;
		}

		private static JVHOnlinePuzzleDto ParsePuzzle(HtmlDocument doc, string puzzleUrl)
		{
			// Parse Title
			HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'product_title entry-title')]");
			string fullTitle = titleNode?.InnerText.Trim() ?? "Unknown Puzzle";

			// Replace HTML entities with their corresponding characters
			fullTitle = System.Net.WebUtility.HtmlDecode(fullTitle);

			// Extract the actual title from the format "brand - title - count"
			string[] titleParts = fullTitle.Split(" – ");
			string title = titleParts.Length > 1 ? titleParts[1] : fullTitle;
			int titlePieceCount = 0;
			if (titleParts.Length > 1)
			{
				string pieceCountString = titleParts[2];
				string[] pieceCountParts = pieceCountString.Split(' ');
				if (pieceCountParts.Length > 0 && int.TryParse(pieceCountParts[0], out int parsedPieceCount))
				{
					titlePieceCount = parsedPieceCount;
				}
			}

			// Parse metadata
			HtmlNode? metadataNode = doc.DocumentNode.SelectSingleNode("//div[@class='product_meta']");
			string tekenaar = metadataNode?.SelectSingleNode(".//div[span[contains(text(), 'Tekenaar')]]/span[@class='content']")?.InnerText.Trim() ?? "Unknown";
			string jaar = metadataNode?.SelectSingleNode(".//div[span[contains(text(), 'Jaar')]]/span[@class='content']")?.InnerText.Trim() ?? "Unknown";
			string stukjes = metadataNode?.SelectSingleNode(".//div[span[contains(text(), 'Stukjes')]]/span[@class='content']")?.InnerText.Trim() ?? "Unknown";
			string artikelnr = metadataNode?.SelectSingleNode(".//div[span[contains(text(), 'Artikelnr.')]]/span[@class='content']")?.InnerText.Trim() ?? "Unknown";

			// Parse Image URL
			HtmlNode imageNode = doc.DocumentNode.SelectSingleNode("//div[@class='woocommerce-product-gallery__image']");
			string imageUrl = imageNode?.GetAttributeValue("data-thumb", "http://some-link.com") ?? "http://some-link.com";

			// parse puzzle details from single puzzle page
			JVHOnlinePuzzleDto puzzleDto = new JVHOnlinePuzzleDto
			{
				Title = title,
				FullTitle = fullTitle,
				PieceCount = int.TryParse(stukjes, out int pieceCount) ? pieceCount : titlePieceCount,
				ProductUrl = puzzleUrl,
				ImageUrl = imageUrl,
				Artist = tekenaar,
				year = int.TryParse(jaar, out int year) ? year : 0,
				articleNumber = int.TryParse(artikelnr, out int articleNumber) ? articleNumber : 0,
			};

			return puzzleDto;
		}
	}
}
