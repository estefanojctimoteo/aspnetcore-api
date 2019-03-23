using MediatR;
using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Core_Api.Domain.Core.Notifications;
using Core_Api.Domain.Handlers;
using Core_Api.Domain.Interfaces;

using Core_Api.Domain.AppUsers.Commands;
using Core_Api.Domain.AppUsers.Events;
using Core_Api.Domain.AppUsers.Repository;

using Core_Api.Infra.CrossCutting.AspNetFilters;
using Core_Api.Infra.CrossCutting.Identity.Models;
using Core_Api.Infra.CrossCutting.Identity.Services;

using Core_Api.Infra.Data.Repository;
using Core_Api.Infra.Data.Context;
using Core_Api.Infra.Data.EventSourcing;
using Core_Api.Infra.Data.Repository.EventSourcing;
using Core_Api.Infra.Data.UoW;

namespace Core_Api.Infra.CrossCutting.IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // ASPNET
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));


            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, MediatorHandler>();


            // Domain - Commands
            services.AddScoped<IRequestHandler<RegisterAppUserCommand>, AppUserCommandHandler>();
            services.AddScoped<IRequestHandler<RemoveAppUserCommand>, AppUserCommandHandler>();


            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<AppUserRegisteredEvent>, AppUserEventHandler>();
            services.AddScoped<INotificationHandler<AppUserRemovedEvent>, AppUserEventHandler>();


            // Infra - Data
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<FirstContext>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddSingleton<ContextManager>();


            // Infra - Data EventSourcing
            services.AddScoped<IEventStoreRepository, EventStoreSQLRepository>();
            services.AddScoped<IEventStore, SqlEventStore>();
            services.AddScoped<EventStoreSQLContext>();


            // Infra - Identity
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IUser, AspNetUser>();


            // Infra - Filters
            services.AddScoped<ILogger<GlobalExceptionHandlingFilter>, Logger<GlobalExceptionHandlingFilter>>();
            services.AddScoped<ILogger<GlobalActionLogger>, Logger<GlobalActionLogger>>();
            services.AddScoped<GlobalExceptionHandlingFilter>();
            services.AddScoped<GlobalActionLogger>();
        }
    }
}