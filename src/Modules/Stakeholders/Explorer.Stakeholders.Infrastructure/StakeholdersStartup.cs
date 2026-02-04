using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Explorer.Stakeholders.Infrastructure.Repositories;
using Fido2NetLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Stakeholders.Infrastructure;

public static class StakeholdersStartup
{
    public static IServiceCollection ConfigureStakeholdersModule(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(StakeholderProfile).Assembly);
        SetupCore(services);
        SetupInfrastructure(services);
        return services;
    }
    
    private static void SetupCore(IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<ITourPreferenceService, TourPreferenceService>();
        services.AddScoped<ITokenGenerator, JwtGenerator>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<IInternalUserService, InternalUserService>();
        services.AddScoped<IInternalJournalService, InternalJournalService>();

        services.AddScoped<IRatingsService, RatingsService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IUserLocationService, UserLocationService>();
        services.AddScoped<IInternalPersonService, PersonService>();
        services.AddScoped<IProblemService, ProblemService>();
        services.AddScoped<IProblemMessageService, ProblemMessageService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IPasskeyService, PasskeyService>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<ITourPreferenceRepository, TourPreferenceDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IRatingRepository, RatingDbRepository>();
        services.AddScoped<IJournalRepository, JournalDbRepository>();
        services.AddScoped<IUserLocationRepository, UserLocationDbRepository>();
        services.AddScoped<IProblemRepository, ProblemDbRepository>();
        services.AddScoped<IProblemMessageRepository, ProblemMessageDbRepository>();
        services.AddScoped<INotificationRepository, NotificationDbRepository>();
        services.AddScoped<IPasskeyCredentialRepository, PasskeyCredentialRepository>();

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));

        services.AddDistributedMemoryCache();

        services.AddFido2(options =>
        {
            options.ServerDomain = Environment.GetEnvironmentVariable("FIDO2_SERVER_DOMAIN") ?? "localhost";
            options.ServerName = "Explorer";
            options.Origins = new HashSet<string>
            {
                Environment.GetEnvironmentVariable("FIDO2_ORIGIN") ?? "http://localhost:4200"
            };
        });
    }
}
