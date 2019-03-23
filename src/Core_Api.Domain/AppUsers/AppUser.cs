using System;
using Core_Api.Domain.Core.Models;

namespace Core_Api.Domain.AppUsers
{
    public class AppUser : EntityUser<AppUser>
    {
        public string Email { get; private set; }        

        public AppUser(Guid userid, string email)
        {
            UserID = userid;
            Email = email;
            Name = "";
        }

        // EF Construtor
        protected AppUser()
        {
        }

        public virtual string Name { get; set; }

        public override bool IsValid()
        {
            return true;
        }
    }
}