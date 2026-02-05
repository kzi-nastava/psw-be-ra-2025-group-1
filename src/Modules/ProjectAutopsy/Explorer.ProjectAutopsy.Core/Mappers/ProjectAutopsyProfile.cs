using AutoMapper;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.Core.Domain;
using Explorer.ProjectAutopsy.Core.Services;

namespace Explorer.ProjectAutopsy.Core.Mappers;

public class ProjectAutopsyProfile : Profile
{
    public ProjectAutopsyProfile()
    {
        // AutopsyProject mappings
        CreateMap<AutopsyProject, AutopsyProjectDto>();
        CreateMap<CreateAutopsyProjectDto, AutopsyProject>();

        // RiskSnapshot mappings
        CreateMap<RiskSnapshot, RiskSnapshotDto>()
            .ForMember(dest => dest.Trend, opt => opt.MapFrom(src => src.Trend.ToString()));

        // GitHubMetrics mappings
        CreateMap<GitHubMetrics, GitHubMetricsDto>();
        CreateMap<CommitAnalysis, CommitAnalysisDto>();
        CreateMap<CommitExample, CommitExampleDto>();
        CreateMap<PullRequestAnalysis, PullRequestAnalysisDto>();
        CreateMap<PrExample, PrExampleDto>();
        CreateMap<ReviewAnalysis, ReviewAnalysisDto>();
        CreateMap<AuthorStats, AuthorStatsDto>();

        // AIReport mappings
        CreateMap<AIReport, AIReportDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<ReportContent, ReportContentDto>();
        CreateMap<KeyFinding, KeyFindingDto>()
            .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Severity.ToString()));
        CreateMap<Recommendation, RecommendationDto>()
            .ForMember(dest => dest.Effort, opt => opt.MapFrom(src => src.Effort.ToString()));
        CreateMap<RiskBreakdown, RiskBreakdownDto>();
    }
}
