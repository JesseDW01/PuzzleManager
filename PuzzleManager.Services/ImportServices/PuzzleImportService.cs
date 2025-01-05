using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PuzzleManager.Data;
using PuzzleManager.Domain;
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
		private readonly IPuzzleScraper _puzzleScraper = puzzleScraper;
		private readonly PuzzleManagerContext _context = context;
		private readonly IMapper _mapper = mapper;

		/// <inheritdoc />
		public async Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm)
		{
			// Use the scraper to search for puzzles matching the search term
			return await _puzzleScraper.SearchPuzzlesAsync(searchTerm);
		}

		/// <inheritdoc />
		public async Task<Puzzle?> ImportPuzzleByUrlAsync(string puzzleUrl, IdentityUser user)
		{
			// Fetch the puzzle DTO from the external website using the scraper
			JVHOnlinePuzzleDto? puzzleDto = await _puzzleScraper.GetPuzzleByUrlAsync(puzzleUrl);

			if (puzzleDto == null)
			{
				// Puzzle not found or scraping failed
				return null;
			}

			// Check if the puzzle already exists in the database to avoid duplicates
			Puzzle? existingPuzzle = await _context.Puzzles.FirstOrDefaultAsync(p => p.ProductUrl == puzzleDto.ProductUrl);
			if (existingPuzzle is not null)
			{
				// Puzzle already exists, optionally handle duplicates
				throw new Exception("Puzzle already exists in the database.");
			}

			// Map the DTO to the domain Puzzle entity using AutoMapper
			Puzzle puzzle = _mapper.Map<Puzzle>(puzzleDto);
			PuzzleMaker maker = await GetOrCreatePuzzleMakerAsync(puzzleDto.Maker);
			puzzle.Maker = maker;
			puzzle.PuzzleMakerId = maker.PuzzleMakerId;

			// Add the new puzzle to the context
			_context.Puzzles.Add(puzzle);

			// Create a new PuzzleCheckout linking the holder to the puzzle
			PuzzleHolder holder = await GetOrCreatePuzzleHolderAsync(user);
			PuzzleCheckout checkout = new PuzzleCheckout
			{
				Puzzle = puzzle,
				PuzzleHolderId = holder.PuzzleHolderId,
				CheckoutDate = DateTime.UtcNow,
				// Initialize other properties as needed
			};
			_context.PuzzleCheckouts.Add(checkout);

			puzzle.CheckoutId = checkout.PuzzleCheckoutId;

			// Save changes to persist the puzzle and checkout in the database
			await _context.SaveChangesAsync();

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
			PuzzleMaker? puzzleMaker = await _context.PuzzleMakers.FirstOrDefaultAsync(m => m.Name == maker);
			if (puzzleMaker != null) 
			{
				return puzzleMaker;
			}

			puzzleMaker = new PuzzleMaker
			{
				Name = maker
			};

			_context.PuzzleMakers.Add(puzzleMaker);
			await _context.SaveChangesAsync(); // Persist to generate PuzzleMakerId

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
			PuzzleHolder? holder = await _context.PuzzleHolders.FirstOrDefaultAsync(h => h.UserId == user.Id);
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

			_context.PuzzleHolders.Add(holder);
			await _context.SaveChangesAsync(); // Persist to generate PuzzleHolderId

			return holder;
		}
	}
}
