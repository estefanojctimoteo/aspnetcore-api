using AutoMapper;
//using Elmah.Io.AspNetCore;
using Core_Api.Infra.CrossCutting.AspNetFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Core_Api.Infra.CrossCutting.Identity.Data;
using Core_Api.Services.Api.Configurations;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Http.Features;
using Hangfire;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Hangfire.Dashboard;
using Hangfire.Annotations;
//using Bugsnag.AspNet.Core;

namespace Core_Api.Services.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-US");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") };
                options.RequestCultureProviders = new List<IRequestCultureProvider>();
            });
            
            // Context of EF to the Identity
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Authentication, authorization and JWT
            services.AddMvcSecurity(Configuration);

            /*
            services.AddHangfire(config =>
                config.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.AddBugsnag(configuration => {
                configuration.ApiKey = "Bugsnag_key_here";
            });
            */

            // Options for customized configurations
            services.AddOptions();

            // MVC restrictions
            services.AddMvc(options =>
            {
                options.OutputFormatters.Remove(new XmlDataContractSerializerOutputFormatter());
                options.Filters.Add(new ServiceFilterAttribute(typeof(GlobalActionLogger)));
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            // WebApi version
            services.AddApiVersioning("api/v{version}");

            // AutoMapper
            services.AddAutoMapper();

            // Swagger
            services.AddSwaggerConfig();

            // MediatR
            services.AddMediatR(typeof(Startup));

            // Registering DI
            services.AddDIConfiguration();
        }

        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              ILoggerFactory loggerFactory,
                              IHttpContextAccessor accessor)
        {

            app.UseRequestLocalization();

            #region Logging with Elmah.IO

            /*

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var elmahSts = new ElmahIoSettings
            {
                OnMessage = message =>
                {
                    message.Version = "v1.0";
                    message.Application = "Core_Api";
                    message.User = accessor.HttpContext.User.Identity.Name;
                },
            };

            loggerFactory.AddElmahIo("elmah_key", new Guid("create_a_new_guid_and_put_it_here"));
            app.UseElmahIo("elmah_key", new Guid("the_same_new_guid_here"), elmahSts);

            */

            #endregion

            #region MVC configurations

            app.UseCors(c =>
            {
                c.AllowAnyHeader();
                c.AllowAnyMethod();
                c.AllowAnyOrigin();
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            #endregion

            #region Swagger

            if (env.IsProduction())
            {
                // uncomment to activate security.
                //app.UseSwaggerAuthorized();
            }

            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "Core_Api v1.0");
            });
            #endregion

            #region Hangfire

            /*
            app.UseHangfireServer();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new MyAuthorizationFilter() }
            });
            */

            #endregion
        }
    }

    public class MyAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // TO_DO: add Authorization properly, because this way
            // allows all authenticated users to see the HangFire Dashboard.
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}
