using AutoMapper;
using Core_Api.Domain.AppUsers.Commands;
using Core_Api.Services.Api.ViewModels;

namespace Core_Api.Services.Api.AutoMapper
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region Usuario

            CreateMap<AppUserViewModel, RemoveAppUserCommand>()
                .ConstructUsing(f => new RemoveAppUserCommand(f.Id));

            #endregion
        }
    }
}

