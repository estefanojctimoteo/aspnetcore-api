using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Core_Api.Services.Api.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Core_Api API",
                    Description = "API do site Core_Api",
                    TermsOfService = "Nenhum",
                    Contact = new Contact { Name = "Desenvolvedor X", Email = "email@Core_Api", Url = "http://Core_Api" },
                    License = new License { Name = "MIT", Url = "http://Core_Api/licensa" }
                });

                s.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            //services.ConfigureSwaggerGen(opt =>
            //{
            //    opt.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            //});
        }
    }
}