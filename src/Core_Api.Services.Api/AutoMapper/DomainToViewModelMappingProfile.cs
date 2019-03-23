using AutoMapper;
using Core_Api.Domain.AppUsers;
using Core_Api.Services.Api.ViewModels;

namespace Core_Api.Services.Api.AutoMapper
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            CreateMap<AppUser, AppUserViewModel>();
        }
    }
}