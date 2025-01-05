// PuzzleManager.Tests/PuzzleImportServiceTests.cs
using PuzzleManager.Services.DTOs;
using PuzzleManager.Services.Scrapers;

namespace PuzzleManager.Tests.PuzzleImport
{
	public class ScraperJanVanHaasterenTests
	{

		[Fact]
		public async Task ScrapePuzzleRobinHood()
		{
			// Arrange
			using HttpClient httpClient = new();
			var scraper = new JanVanHaasterenScraper(httpClient);
			string testPuzzleUrl = "https://janvanhaasteren.nl/puzzel/jan-van-haasteren-robin-hood-festival-1500-stukjes-puzzel/";

			// Act
			JVHOnlinePuzzleDto? puzzleDto = await scraper.GetPuzzleByUrlAsync(testPuzzleUrl);

			// Assert
			Assert.NotNull(puzzleDto);
			Assert.Equal("Robin Hood Festival", puzzleDto.Title);
			Assert.Equal("Jan van Haasteren – Robin Hood Festival – 1500 stukjes puzzel", puzzleDto.FullTitle);
			Assert.Equal(1500, puzzleDto.PieceCount);
			Assert.Equal(1110100317, puzzleDto.ArticleNumber);
			Assert.Equal("Rob Derks", puzzleDto.Artist);
			Assert.Equal(2024, puzzleDto.Year);
			Assert.Equal("https://janvanhaasteren.nl/wp-content/uploads/2024/08/1110100317_L_0_WEB.png", puzzleDto.ImageUrl);
			Assert.Equal(testPuzzleUrl, puzzleDto.ProductUrl);
		}

		[Fact]
		public async Task ScrapePuzzleDeVerbouwingAndDeBouwmarkt()
		{
			// Arrange
			using HttpClient httpClient = new();
			var scraper = new JanVanHaasterenScraper(httpClient);
			string testPuzzleUrl = "https://janvanhaasteren.nl/puzzel/jan-van-haasteren-de-verbouwing-de-bouwmarkt-2x1000-stukjes/";

			// Act
			JVHOnlinePuzzleDto? puzzleDto = await scraper.GetPuzzleByUrlAsync(testPuzzleUrl);

			// Assert
			Assert.NotNull(puzzleDto);
			Assert.Equal("De verbouwing & De bouwmarkt", puzzleDto.Title);
			Assert.Equal("Jan van Haasteren – De verbouwing & De bouwmarkt – 2×1000 stukjes", puzzleDto.FullTitle);
			Assert.Equal(1000, puzzleDto.PieceCount);
			Assert.Equal(1110100325, puzzleDto.ArticleNumber);
			Assert.Equal("Rob Derks", puzzleDto.Artist);
			Assert.Equal(2024, puzzleDto.Year);
			Assert.Equal("https://janvanhaasteren.nl/wp-content/uploads/2024/11/1110100325_L_0.png", puzzleDto.ImageUrl);
			Assert.Equal(testPuzzleUrl, puzzleDto.ProductUrl);
		}
	}

}
