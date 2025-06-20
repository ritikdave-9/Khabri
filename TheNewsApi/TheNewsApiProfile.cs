using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entity;


namespace TheNewsApi
{
    public class TheNewsApiProfile:Profile
    {
        public TheNewsApiProfile()
        {
            CreateMap<TheNewApiResponseDto, News>()
                .ForMember(dest => dest.NewsID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title ?? string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url ?? string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl ?? string.Empty))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Snippet ?? string.Empty))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Source ?? string.Empty))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => string.Empty)) 
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language ?? string.Empty))
                .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.PublishedAt))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
