using Core_Api.Infra.CrossCutting.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core_Api.Services.Api.Configurations
{
    public static class DependencyInjectionConfiguration
    {
        public static void AddDIConfiguration(this IServiceCollection services)
        {
            NativeInjectorBootStrapper.RegisterServices(services);
        }
    }
}