using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Mappers;
using Explorer.Stakeholders.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Infrastructure.Database.Repositories;
using Explorer.Stakeholders.Infrastructure.Repositories;
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
        services.AddScoped<ITokenGenerator, JwtGenerator>();
<<<<<<< HEAD
        services.AddScoped<IMessageService, MessageService>();
=======

        services.AddScoped<IRatingsService, RatingsService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<IUserLocationService, UserLocationService>();
>>>>>>> development
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        services.AddScoped<IPersonRepository, PersonDbRepository>();
        services.AddScoped<IUserRepository, UserDbRepository>();

<<<<<<< HEAD
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
=======
        services.AddScoped<IRatingRepository, RatingDbRepository>();
        services.AddScoped<IUserLocationRepository, UserLocationDbRepository>();
>>>>>>> development

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("stakeholders"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        
        services.AddDbContext<StakeholdersContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "stakeholders")));
    }
}
