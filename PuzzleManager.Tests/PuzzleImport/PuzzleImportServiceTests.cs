using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using PuzzleManager.Data;
using PuzzleManager.Domain.Entities;
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
			MapperConfiguration config = new(cfg =>
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
			Mock<IPuzzleScraper> mockScraper = new();
			string puzzleUrl = "https://janvanhaasteren.nl/puzzel/test-puzzle/";
			JVHOnlinePuzzleDto testDto = new()
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
			DbContextOptions<PuzzleManagerContext> options = new DbContextOptionsBuilder<PuzzleManagerContext>()
				.UseInMemoryDatabase(databaseName: "ImportPuzzle_Success")
				.Options;

			using PuzzleManagerContext context = new(options);

			// Create a test user
			IdentityUser testUser = new()
			{
				Id = "user-123",
				UserName = "testuser",
				Email = "testuser@example.com"
			};

			// Initialize the service
			PuzzleImportService service = new(mockScraper.Object, context, _mapper);

			// Act
			Puzzle? puzzle = await service.ImportPuzzleByUrlAsync(puzzleUrl, testUser);

			// Assert
			Assert.NotNull(puzzle);
			Assert.Equal(testDto.Title, puzzle.Name);
			Assert.Equal(testDto.PieceCount, puzzle.PieceCount);
			Assert.Equal(testDto.ProductUrl, puzzle.ProductUrl);
			Assert.Equal(testDto.ImageUrl, puzzle.ImageUrl);
			Assert.Equal(testDto.Year, puzzle.Year);
			Assert.Equal(testDto.ArticleNumber, puzzle.ArticleNumber);
			Assert.Equal(testDto.Artist, puzzle.Artist);

			// Verify Maker was created and linked
			PuzzleMaker? Maker = await context.PuzzleMakers.FirstOrDefaultAsync(m => m.PuzzleMakerId == puzzle.PuzzleMakerId);
			Assert.NotNull(Maker);
			Assert.Equal(Maker.Name, testDto.Maker);
			Assert.Equal(puzzle.PuzzleMakerId, Maker.PuzzleMakerId);
			Assert.Contains(puzzle, Maker.Puzzles);

			// Verify PuzzleCheckout was created and linked
			PuzzleCheckout? checkout = await context.PuzzleCheckouts.FirstOrDefaultAsync(c => c.PuzzleId == puzzle.PuzzleId);
			Assert.NotNull(checkout);
			Assert.Equal(DateTime.UtcNow.Date, checkout.CheckoutDate.Date);
			Assert.Equal(puzzle.PuzzleId, checkout.PuzzleId);
			Assert.Equal(puzzle, checkout.Puzzle);
			Assert.Contains(checkout, puzzle.PuzzleCheckouts);

			// Verify PuzzleHolder was created and linked
			PuzzleHolder? holder = await context.PuzzleHolders.FirstOrDefaultAsync(h => h.UserId == testUser.Id);
			Assert.NotNull(holder);
			Assert.Equal(testUser.UserName, holder.Name);
			Assert.Equal(holder.PuzzleHolderId, checkout.PuzzleHolderId);
			Assert.Equal(holder, checkout.PuzzleHolder);
			Assert.Contains(checkout, holder.PuzzleCheckouts);

		}

		/// <summary>
		/// Tests importing a puzzle that already exists in the database and throws an exception.
		/// Ensures that the exception is thrown and the message is correct.
		/// </summary>
		[Fact]
		public async Task ImportPuzzleByUrlAsync_ShouldThrowException_WhenPuzzleAlreadyExists()
		{
			// Arrange
			Mock<IPuzzleScraper> mockScraper = new();
			string puzzleUrl = "https://janvanhaasteren.nl/puzzel/existing-puzzle/";
			JVHOnlinePuzzleDto testDto = new()
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
			DbContextOptions<PuzzleManagerContext> options = new DbContextOptionsBuilder<PuzzleManagerContext>()
				.UseInMemoryDatabase(databaseName: "ImportPuzzle_Exception")
				.Options;

			using PuzzleManagerContext context = new(options);

			// Add existing puzzle
			Puzzle existingPuzzle = new()
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
			IdentityUser testUser = new()
			{
				Id = "user-456",
				UserName = "anotheruser",
				Email = "anotheruser@example.com"
			};

			// Initialize the service
			PuzzleImportService service = new(mockScraper.Object, context, _mapper);

			// Act & Assert
			Exception exception = await Assert.ThrowsAsync<Exception>(() => service.ImportPuzzleByUrlAsync(puzzleUrl, testUser));
			Assert.Equal("Puzzle already exists in the database.", exception.Message);
		}
	}
}
