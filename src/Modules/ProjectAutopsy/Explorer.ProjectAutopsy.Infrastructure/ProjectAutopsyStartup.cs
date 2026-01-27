using Explorer.ProjectAutopsy.API.Public;
using Explorer.ProjectAutopsy.Core.Domain.RepositoryInterfaces;
using Explorer.ProjectAutopsy.Core.Mappers;
using Explorer.ProjectAutopsy.Core.Services;
using Explorer.ProjectAutopsy.Core.UseCases;
using Explorer.ProjectAutopsy.Infrastructure.Database;
using Explorer.ProjectAutopsy.Infrastructure.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.ProjectAutopsy.Infrastructure;

public static class ProjectAutopsyStartup
{
    public static IServiceCollection ConfigureProjectAutopsyModule(this IServiceCollection services, string connectionString)
    {
        // Database context
        services.AddDbContext<ProjectAutopsyContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories
        services.AddScoped<IAutopsyProjectRepository, AutopsyProjectRepository>();
        services.AddScoped<IRiskSnapshotRepository, RiskSnapshotRepository>();
        services.AddScoped<IAIReportRepository, AIReportRepository>();

        // Services
        services.AddScoped<IAutopsyProjectService, AutopsyProjectService>();
        services.AddScoped<IRiskAnalysisService, RiskAnalysisService>();
        
        // Risk Engine (singleton - stateless)
        services.AddSingleton<RiskEngine>();
        
        // LLM Client (use mock by default, can be replaced with real implementation)
        services.AddSingleton<ILLMClient, MockLLMClient>();

        // AutoMapper
        services.AddAutoMapper(typeof(ProjectAutopsyProfile).Assembly);

        return services;
    }
}
