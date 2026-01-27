using AutoMapper;
using Explorer.ProjectAutopsy.API.Dtos;
using Explorer.ProjectAutopsy.Core.Domain;

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

        CreateMap<RiskMetrics, RiskMetricsDto>();
        CreateMap<PlanningMetrics, PlanningMetricsDto>();
        CreateMap<ExecutionMetrics, ExecutionMetricsDto>();
        CreateMap<BottleneckMetrics, BottleneckMetricsDto>();
        CreateMap<CommunicationMetrics, CommunicationMetricsDto>();
        CreateMap<StabilityMetrics, StabilityMetricsDto>();

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
