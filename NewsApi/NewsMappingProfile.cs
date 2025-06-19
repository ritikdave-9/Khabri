using AutoMapper;
using Data.Entity;

namespace NewsApi
{
    public class NewsMappingProfile : Profile
    {
        public NewsMappingProfile()
        {
            CreateMap<NewsArticle, News>()
                .ForMember(dest => dest.NewsID, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.UrlToImage))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Source.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(_ => "en"))
                .ForMember(dest => dest.NewsSourceId, opt => opt.MapFrom(_ => Guid.NewGuid())); // optionally map with actual logic
        }
    }
}
