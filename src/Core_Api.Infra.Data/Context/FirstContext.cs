using System.IO;
using Core_Api.Infra.Data.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Core_Api.Infra.Data.Mappings;
using Core_Api.Domain.AppUsers;

namespace Core_Api.Infra.Data.Context
{
    public class FirstContext : DbContext
    {
        static FirstContext()
        {
            
        }

        public DbSet<AppUser> AppUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddConfiguration(new AppUserMapping());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }
}