using PuzzleManager.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleManager.Services
{
	internal interface IPuzzleScraper
	{
		Task<JVHOnlinePuzzleDto?> GetPuzzleByUrlAsync(string puzzleUrl);
		Task<List<JVHOnlinePuzzleDto>> SearchPuzzlesAsync(string searchTerm);
	}
}
