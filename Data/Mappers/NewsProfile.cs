using AutoMapper;
using Common.Dtos;
using Data.Entity;
using Common.Dtos;

namespace Data.Mappers
{
    public class NewsProfile : Profile
    {
        public NewsProfile()
        {
            CreateMap<News, NewsResponseDto>();
        }
    }
}