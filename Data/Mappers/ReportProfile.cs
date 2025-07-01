using AutoMapper;
using Common.Dtos;
using Data.Entity;

namespace Data.Mappers
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<Report, ReportResponseDto>()
                .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Reporter.FirstName + " " + src.Reporter.LastName))
                .ForMember(dest => dest.NewsTitle, opt => opt.MapFrom(src => src.News.Title));
        }
    }
}