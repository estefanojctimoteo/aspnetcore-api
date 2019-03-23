using Microsoft.AspNetCore.Http;
using System;
using Core_Api.Domain.Interfaces;

namespace Core_Api.Infra.Data.UoW
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly IServiceProvider _servicesProvider;

        public UnitOfWorkFactory(IHttpContextAccessor httpContextAccessor, IServiceProvider servicesProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _servicesProvider = servicesProvider;
        }

        public IUnitOfWork Create()
        {
            return new UnitOfWork(_httpContextAccessor, _servicesProvider);
        }
    }
}
