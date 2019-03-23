using Core_Api.Domain.Interfaces;
using System.Security.Claims;
using System.Collections.Generic;

namespace Core_Api.Domain.AppUsers.Repository
{
    public interface IAppUserRepository : IRepositoryUser<AppUser>
    {
        IEnumerable<Claim> GetAllClaimsType();
    }
}