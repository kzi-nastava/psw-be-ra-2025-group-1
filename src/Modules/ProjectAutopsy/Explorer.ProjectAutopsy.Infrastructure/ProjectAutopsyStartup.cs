using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.ProjectAutopsy.API.Public;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Mappers;
using Explorer.ProjectAutopsy.Core.Services;
using Explorer.ProjectAutopsy.Core.UseCases;
using Explorer.ProjectAutopsy.Infrastructure.Database;
using Explorer.ProjectAutopsy.Infrastructure.Database.Repositories;
using Explorer.ProjectAutopsy.Infrastructure.ExternalClients;
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

        SetupCore(services);
        SetupInfrastructure(services, configuration);

        return services;
    }

    private static void SetupCore(IServiceCollection services)
    {
        // Services
        services.AddScoped<IAutopsyProjectService, AutopsyProjectService>();
        services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();
        
        // Risk Engine (singleton - stateless)
        services.AddSingleton<RiskEngine>();
        
        // LLM Client (use mock by default, can be replaced with real implementation)
        services.AddSingleton<ILLMClient, MockLLMClient>();
    }

    private static void SetupInfrastructure(IServiceCollection services, IConfiguration configuration)
    {
        // Repositories
        services.AddScoped<IAutopsyProjectRepository, AutopsyProjectRepository>();
        services.AddScoped<IRiskSnapshotRepository, RiskSnapshotRepository>();
        services.AddScoped<IAIReportRepository, AIReportRepository>();

        // GitHub Client - reads token from environment variable
        var githubToken = configuration["GITHUB_TOKEN"] ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        services.AddSingleton<GitHubClient>(sp => new GitHubClient(githubToken ?? string.Empty));

        // GitHub Data Service - wraps GitHubClient with interface for Core layer
        services.AddScoped<IGitHubDataService, GitHubDataService>();

        // PDF Export Service
        services.AddScoped<IPdfExportService, PdfExportService>();

        // Database context with proper connection string
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("autopsy"));
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();

        services.AddDbContext<ProjectAutopsyContext>(opt =>
            opt.UseNpgsql(dataSource,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "autopsy")));
    }
}
