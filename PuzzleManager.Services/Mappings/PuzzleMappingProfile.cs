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
				// properties with the same name are automatically mapped
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.FullTitle))

				// Ignore following proprties as tyet will be assigned trough services surrounding
				.ForMember(dest => dest.PuzzleId, opt => opt.Ignore())
				.ForMember(dest => dest.PuzzleMakerId, opt => opt.Ignore())
				.ForMember(dest => dest.Maker, opt => opt.Ignore())
				.ForMember(dest => dest.CheckoutId, opt => opt.Ignore())
				.ForMember(dest => dest.PuzzleCheckouts, opt => opt.Ignore())
				.ForMember(dest => dest.DifficultyRating, opt => opt.Ignore())
				;
		}
	}
}
