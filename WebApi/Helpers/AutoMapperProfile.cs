using AutoMapper;
using WebApi.Dtos;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<Interview, InterviewDto>();
            CreateMap<InterviewDto, Interview>();

            CreateMap<Question, QuestionDto>();
            CreateMap<QuestionDto, Question>();

            CreateMap<Application, ApplicationDto>();
            CreateMap<ApplicationDto, Application>();

            CreateMap<Application, ActiveInteriewDto>();
            CreateMap<ActiveInteriewDto, Application>();
        }
    }
}