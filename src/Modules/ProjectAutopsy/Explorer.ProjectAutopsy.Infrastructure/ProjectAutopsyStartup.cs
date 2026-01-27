using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.ProjectAutopsy.API.Public;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Mappers;
using Explorer.ProjectAutopsy.Core.Services;
using Explorer.ProjectAutopsy.Core.UseCases;
using Explorer.ProjectAutopsy.Infrastructure.Database;
using Explorer.ProjectAutopsy.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.ProjectAutopsy.Infrastructure;

public static class ProjectAutopsyStartup
{
    public static IServiceCollection ConfigureProjectAutopsyModule(this IServiceCollection services, IConfiguration configuration)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(ProjectAutopsyProfile).Assembly);
        
        SetupCore(services, configuration);
        SetupInfrastructure(services);

        return services;
    }

    private static void SetupCore(IServiceCollection services, IConfiguration configuration)
    {
        // Services
        services.AddScoped<IAutopsyProjectService, AutopsyProjectService>();
        services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();
        
        // Risk Engine (singleton - stateless)
        services.AddSingleton<RiskEngine>();
        
        // LLM Client (use mock by default, can be replaced with real implementation)
        // When you want to use a real GitHub API client, uncomment below and implement GitHubLLMClient
        // var gitHubToken = configuration["ProjectAutopsy:GitHubToken"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        // if (!string.IsNullOrEmpty(gitHubToken))
        // {
        //     services.AddSingleton<ILLMClient>(sp => new GitHubLLMClient(gitHubToken));
        // }
        // else
        // {
        //     services.AddSingleton<ILLMClient, MockLLMClient>();
        // }
        
        services.AddSingleton<ILLMClient, MockLLMClient>();
    }

    private static void SetupInfrastructure(IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IAutopsyProjectRepository, AutopsyProjectRepository>();
        services.AddScoped<IRiskSnapshotRepository, RiskSnapshotRepository>();
        services.AddScoped<IAIReportRepository, AIReportRepository>();

        // Database context with proper connection string
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("autopsy"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ProjectAutopsyContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "autopsy")));
    }
}
