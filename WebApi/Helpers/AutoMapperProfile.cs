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

            CreateMap<Admin, AdminDto>();
            CreateMap<AdminDto, Admin>();

            CreateMap<Template, TemplateDto>();
            CreateMap<TemplateDto, Template>();

            CreateMap<TemplateQuestion, QuestionDto>();
            CreateMap<QuestionDto, TemplateQuestion>();

            CreateMap<ApplicationQuestion, ApplicationQuestionDto>();
            CreateMap<ApplicationQuestionDto, ApplicationQuestion>();

            CreateMap<Application, ApplicationDto>();
            CreateMap<ApplicationDto, Application>();

            CreateMap<Application, ActiveInteriewDto>();
            CreateMap<ActiveInteriewDto, Application>();
        }
    }
}