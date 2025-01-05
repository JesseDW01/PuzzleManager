using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PuzzleManager.Data;
using PuzzleManager.Domain.Entities;
using PuzzleManager.Services.DTOs;
using PuzzleManager.Services.Interfaces;

namespace PuzzleManager.Services.ImportServices
{
	/// <summary>
	/// Service responsible for importing puzzles from external sources.
	/// </summary>
	/// <remarks>
	/// Initializes a new instance of the PuzzleImportService class.
	/// </remarks>
	/// <param name="puzzleScraper">The scraper used to fetch puzzle data.</param>
	/// <param name="context">The database context for puzzle management.</param>
	/// <param name="mapper">The AutoMapper instance for mapping DTOs to domain models.</param>
	public class PuzzleImportService(IPuzzleScraper puzzleScraper, PuzzleManagerContext context, IMapper mapper) : IPuzzleImportService
	{

		/// <inheritdoc />
		public async Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm)
		{
			// Use the scraper to search for puzzles matching the search term
			return await puzzleScraper.SearchPuzzlesAsync(searchTerm);
		}

		/// <inheritdoc />
		public async Task<Puzzle?> ImportPuzzleByUrlAsync(string puzzleUrl, IdentityUser user)
		{
			// Fetch the puzzle DTO from the external website using the scraper
			JVHOnlinePuzzleDto? puzzleDto = await puzzleScraper.GetPuzzleByUrlAsync(puzzleUrl);

			if (puzzleDto == null)
			{
				// Puzzle not found or scraping failed
				return null;
			}

			// Check if the puzzle already exists in the database to avoid duplicates
			Puzzle? existingPuzzle = await context.Puzzles.FirstOrDefaultAsync(p => p.ProductUrl == puzzleDto.ProductUrl);
			if (existingPuzzle is not null)
			{
				// Puzzle already exists, optionally handle duplicates
				throw new Exception("Puzzle already exists in the database.");
			}

			Puzzle puzzle = mapper.Map<Puzzle>(puzzleDto);
			context.Puzzles.Add(puzzle); // Add puzzle to get the puzzle ID by the ETF Core

			// Fetch the puzzle maker from the database or create a new one and link it to the puzzle
			PuzzleMaker maker = await GetOrCreatePuzzleMakerAsync(puzzleDto.Maker);
			puzzle.Maker = maker;
			puzzle.PuzzleMakerId = maker.PuzzleMakerId;
			maker.Puzzles.Add(puzzle);

			// Fetch the puzzle holder from the database or create a new one and link it to a new puzzlecheckout
			PuzzleHolder holder = await GetOrCreatePuzzleHolderAsync(user);
			PuzzleCheckout checkout = new()
			{
				Puzzle = puzzle,
				PuzzleHolderId = holder.PuzzleHolderId,
				PuzzleHolder = holder,
				CheckoutDate = DateTime.UtcNow,
			};
			context.PuzzleCheckouts.Add(checkout);

			holder.PuzzleCheckouts.Add(checkout);
			puzzle.CheckoutId = checkout.PuzzleCheckoutId;

			// Save changes to persist the puzzle and checkout in the database
			await context.SaveChangesAsync();

			return puzzle;
		}

		/// <summary>
		/// Retrieves an existing PuzzleHolder by userId or creates a new one if not found.
		/// </summary>
		/// <param name="maker">The Identity User ID.</param>
		/// <returns>The existing or newly created PuzzleHolder.</returns>
		private async Task<PuzzleMaker> GetOrCreatePuzzleMakerAsync(string maker)
		{
			// Attempt to find an existing PuzzleHolder linked to the userId
			PuzzleMaker? puzzleMaker = await context.PuzzleMakers.FirstOrDefaultAsync(m => m.Name == maker);
			if (puzzleMaker != null) 
			{
				return puzzleMaker;
			}

			puzzleMaker = new PuzzleMaker
			{
				Name = maker
			};

			context.PuzzleMakers.Add(puzzleMaker);
			await context.SaveChangesAsync(); // Persist to generate PuzzleMakerId

			return puzzleMaker;
		}

		/// <summary>
		/// Retrieves an existing PuzzleHolder by userId or creates a new one if not found.
		/// </summary>
		/// <param name="userId">The Identity User ID.</param>
		/// <returns>The existing or newly created PuzzleHolder.</returns>
		private async Task<PuzzleHolder> GetOrCreatePuzzleHolderAsync(IdentityUser user)
		{
			// Attempt to find an existing PuzzleHolder linked to the userId
			PuzzleHolder? holder = await context.PuzzleHolders.FirstOrDefaultAsync(h => h.UserId == user.Id);
			if (holder != null)
			{
				return holder;
			}

			// If not found, create a new PuzzleHolder
			holder = new PuzzleHolder
			{
				Name = user.UserName ?? user.Email!,
				UserId = user.Id
				// Set other properties if necessary
			};

			context.PuzzleHolders.Add(holder);
			await context.SaveChangesAsync(); // Persist to generate PuzzleHolderId

			return holder;
		}
	}
}
