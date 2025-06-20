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
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.UrlToImage ?? string.Empty))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Source.Name ?? string.Empty))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(_ => "en"))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url ?? string.Empty))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content ?? string.Empty))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author ?? string.Empty))
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt));
        }

    }
}
