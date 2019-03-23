using System;
using System.Collections.Generic;
using System.Security.Claims;
using Core_Api.Domain.AppUsers;
using Core_Api.Domain.AppUsers.Repository;
using Core_Api.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using Dapper;

namespace Core_Api.Infra.Data.Repository
{
    public class AppUserRepository : RepositoryAppUser<AppUser>, IAppUserRepository
    {
        public AppUserRepository(FirstContext context) : base(context)
        {

        }
        public IEnumerable<Claim> GetAllClaimsType()
        {
            var sql = "select DISTINCT ClaimType as type, ClaimValue as value " +
                      "from AspNetUserClaims (nolock) " +
                      "order by type, value ";
            return Db.Database.GetDbConnection().Query<Claim>(sql);
        }

        public override void Remove(Guid id)
        {
            var appUser = GetById(id);
            appUser.ExecuteDeletion();
            Update(appUser);
        }
    }
}
