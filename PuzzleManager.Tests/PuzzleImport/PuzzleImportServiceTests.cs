using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PuzzleManager.Data;
using PuzzleManager.Domain;
using PuzzleManager.Services.DTOs;
using PuzzleManager.Services.ImportServices;
using PuzzleManager.Services.Interfaces;
using PuzzleManager.Services.Mappings;

namespace PuzzleManager.Tests.PuzzleImport
{
	public class PuzzleImportServiceTests
	{
		private readonly IMapper _mapper;
		public PuzzleImportServiceTests()
		{
			// Initialize AutoMapper configuration
			var config = new MapperConfiguration(cfg =>
			{
				cfg.AddProfile<PuzzleMappingProfile>();
			});

			// Assert AutoMapper configuration is valid
			try
			{
				config.AssertConfigurationIsValid();
			}
			catch (AutoMapperConfigurationException ex)
			{
				// Output the error message to help diagnose the issue
				Console.WriteLine(ex.Message);
				throw;
			}

			// Create the IMapper instance
			_mapper = config.CreateMapper();
		}

		/// <summary>
		/// Tests importing a puzzle successfully when the puzzle does not already exist.
		/// Ensures that the puzzle is added, A maker is added, the holder is linked, and a PuzzleCheckout is created.
		/// </summary>
		[Fact]
		public async Task ImportPuzzleByUrlAsync_ShouldAddPuzzleAndCheckout_WhenPuzzleDoesNotExist()
		{
			// Arrange
			var mockScraper = new Mock<IPuzzleScraper>();
			var puzzleUrl = "https://janvanhaasteren.nl/puzzel/test-puzzle/";
			var testDto = new JVHOnlinePuzzleDto
			{
				Title = "Test Puzzle",
				FullTitle = "Jan van Haasteren – Test Puzzle – 1000 stukjes puzzel",
				PieceCount = 1000,
				ProductUrl = puzzleUrl,
				ImageUrl = "https://janvanhaasteren.nl/images/test-puzzle.png",
				Artist = "Rob Derks",
				Year = 2024,
				ArticleNumber = 123456789
			};

			mockScraper.Setup(s => s.GetPuzzleByUrlAsync(puzzleUrl))
					   .ReturnsAsync(testDto);

			// Setup in-memory database
			var options = new DbContextOptionsBuilder<PuzzleManagerContext>()
				.UseInMemoryDatabase(databaseName: "ImportPuzzle_Success")
				.Options;

			using var context = new PuzzleManagerContext(options);

			// Create a test user
			var testUser = new IdentityUser
			{
				Id = "user-123",
				UserName = "testuser",
				Email = "testuser@example.com"
			};

			// Initialize the service
			var service = new PuzzleImportService(mockScraper.Object, context, _mapper);

			// Act
			Puzzle? result = await service.ImportPuzzleByUrlAsync(puzzleUrl, testUser);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(testDto.Title, result.Name);
			Assert.Equal(testDto.PieceCount, result.PieceCount);
			Assert.Equal(testDto.ProductUrl, result.ProductUrl);

			// Verify PuzzleHolder was created
			var holder = await context.PuzzleHolders.FirstOrDefaultAsync(h => h.UserId == testUser.Id);
			Assert.NotNull(holder);
			Assert.Equal(testUser.UserName, holder.Name);

			// Verify PuzzleCheckout was created
			var checkout = await context.PuzzleCheckouts.FirstOrDefaultAsync(c => c.PuzzleHolderId == holder.PuzzleHolderId && c.PuzzleId == result.PuzzleId);
			Assert.NotNull(checkout);
			Assert.Equal(DateTime.UtcNow.Date, checkout.CheckoutDate.Date);

			// Verify Maker was created
			var Maker = await context.PuzzleMakers.FirstOrDefaultAsync(m => m.PuzzleMakerId == result.PuzzleMakerId);
			Assert.NotNull(Maker);
			Assert.Equal(Maker.Name, testDto.Maker);
		}

		/// <summary>
		/// Tests importing a puzzle that already exists in the database and throws an exception.
		/// Ensures that the exception is thrown and the message is correct.
		/// </summary>
		[Fact]
		public async Task ImportPuzzleByUrlAsync_ShouldThrowException_WhenPuzzleAlreadyExists()
		{
			// Arrange
			var mockScraper = new Mock<IPuzzleScraper>();
			var puzzleUrl = "https://janvanhaasteren.nl/puzzel/existing-puzzle/";
			var testDto = new JVHOnlinePuzzleDto
			{
				Title = "Existing Puzzle",
				FullTitle = "Jan van Haasteren – Existing Puzzle – 1500 stukjes puzzel",
				PieceCount = 1500,
				ProductUrl = puzzleUrl,
				ImageUrl = "https://janvanhaasteren.nl/images/existing-puzzle.png",
				Artist = "Rob Derks",
				Year = 2024,
				ArticleNumber = 987654321
			};

			mockScraper.Setup(s => s.GetPuzzleByUrlAsync(puzzleUrl))
					   .ReturnsAsync(testDto);

			// Setup in-memory database with existing puzzle
			var options = new DbContextOptionsBuilder<PuzzleManagerContext>()
				.UseInMemoryDatabase(databaseName: "ImportPuzzle_Exception")
				.Options;

			using var context = new PuzzleManagerContext(options);

			// Add existing puzzle
			var existingPuzzle = new Puzzle
			{
				PuzzleId = 1,
				Name = testDto.Title,
				Description = testDto.FullTitle,
				PieceCount = testDto.PieceCount,
				ProductUrl = testDto.ProductUrl,
				ImageUrl = testDto.ImageUrl,
				Year = testDto.Year ?? 0,
				ArticleNumber = testDto.ArticleNumber,
				PuzzleMakerId = 1,
				CheckoutId = 1,
				Artist = "Existing Artist"
			};
			context.Puzzles.Add(existingPuzzle);
			await context.SaveChangesAsync();

			// Create a test user
			var testUser = new IdentityUser
			{
				Id = "user-456",
				UserName = "anotheruser",
				Email = "anotheruser@example.com"
			};

			// Initialize the service
			var service = new PuzzleImportService(mockScraper.Object, context, _mapper);

			// Act & Assert
			var exception = await Assert.ThrowsAsync<Exception>(() => service.ImportPuzzleByUrlAsync(puzzleUrl, testUser));
			Assert.Equal("Puzzle already exists in the database.", exception.Message);
		}
	}
}
