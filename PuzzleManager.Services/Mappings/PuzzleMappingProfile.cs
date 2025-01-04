using AutoMapper;
using PuzzleManager.Domain;
using PuzzleManager.Services.DTOs;

namespace PuzzleManager.Services.Mappings
{
	/// <summary>
	/// Defines AutoMapper profiles for mapping between DTOs and domain models.
	/// </summary>
	public class PuzzleMappingProfile : Profile
	{
		public PuzzleMappingProfile()
		{
			// Map JVHOnlinePuzzleDto to Puzzle domain model
			CreateMap<JVHOnlinePuzzleDto, Puzzle>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.FullTitle))
				.ForMember(dest => dest.PieceCount, opt => opt.MapFrom(src => src.PieceCount))
				.ForMember(dest => dest.ProductUrl, opt => opt.MapFrom(src => src.ProductUrl))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.year))
				.ForMember(dest => dest.ArticleNumber, opt => opt.MapFrom(src => src.articleNumber))
				// PuzzleMakerId will be set in the service after ensuring the maker exists
				.ForMember(dest => dest.PuzzleMakerId, opt => opt.MapFrom(src => src.Maker));
		}
	}
}
