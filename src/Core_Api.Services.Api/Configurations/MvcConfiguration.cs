using System;
using Core_Api.Infra.CrossCutting.Identity.Authorization;
using Core_Api.Infra.CrossCutting.Identity.Data;
using Core_Api.Infra.CrossCutting.Identity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core_Api.Services.Api.Configurations
{
    public static class MvcConfiguration
    {
        public static void AddMvcSecurity(this IServiceCollection services, IConfigurationRoot configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            var tokenConfigurations = new TokenDescriptor();
            new ConfigureFromConfigurationOptions<TokenDescriptor>(
                    configuration.GetSection("JwtTokenOptions"))
                .Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                bearerOptions.RequireHttpsMetadata = false;
                bearerOptions.SaveToken = true;

                var paramsValidation = bearerOptions.TokenValidationParameters;

                paramsValidation.IssuerSigningKey = SigningCredentialsConfiguration.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;

                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.ValidateLifetime = true;
                paramsValidation.ClockSkew = TimeSpan.Zero;

            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanReadUsers", policy => policy.RequireClaim("AppUsers", "Ler"));
                options.AddPolicy("CanUpdateUsers", policy => policy.RequireClaim("AppUsers", "Gravar"));

                options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }
    }
}