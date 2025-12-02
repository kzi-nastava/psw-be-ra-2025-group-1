using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Administration;
using DomainProblemStatus = Explorer.Stakeholders.Core.Domain.ProblemStatus;
using DtoProblemStatus = Explorer.Stakeholders.API.Dtos.ProblemStatus;
using DomainProblemCategory = Explorer.Stakeholders.Core.Domain.ProblemCategory;

namespace Explorer.Stakeholders.Core.UseCases;

public class ProblemService : IProblemService
{
    private readonly IProblemRepository _problemRepository;
    private readonly ITourService _tourService;
    private readonly IMapper _mapper;

    public ProblemService(IProblemRepository repository, ITourService tourService, IMapper mapper)
    {
        _problemRepository = repository;
        _tourService = tourService;
        _mapper = mapper;
    }

    public PagedResult<ProblemDto> GetPaged(int page, int pageSize)
    {
        var result = _problemRepository.GetPaged(page, pageSize);
        var items = result.Results.Select(_mapper.Map<ProblemDto>).ToList();
        return new PagedResult<ProblemDto>(items, result.TotalCount);
    }

    public PagedResult<ProblemDto> GetByCreator(long creatorId, int page, int pageSize)
    {
        var result = _problemRepository.GetByCreatorId(creatorId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<ProblemDto>).ToList();
        return new PagedResult<ProblemDto>(items, result.TotalCount);
    }

    public PagedResult<ProblemDto> GetByAuthor(long authorId, int page, int pageSize)
    {
        Console.WriteLine($"🔍 GetByAuthor called with authorId={authorId}");
        var result = _problemRepository.GetByAuthorId(authorId, page, pageSize);
        Console.WriteLine($"🔍 Found {result.Results.Count} problems for author {authorId}");
        
        foreach (var problem in result.Results)
        {
            Console.WriteLine($"   - Problem Id={problem.Id}, TourId={problem.TourId}, AuthorId={problem.AuthorId}, CreatorId={problem.CreatorId}");
        }
        
        var items = result.Results.Select(_mapper.Map<ProblemDto>).ToList();
        return new PagedResult<ProblemDto>(items, result.TotalCount);
    }

    public ProblemDto Get(long id, long userId)
    {
        var problem = _problemRepository.Get(id);

        if (userId != problem.CreatorId && userId != problem.AuthorId)
            throw new UnauthorizedAccessException("Only participants can view this problem.");

        return _mapper.Map<ProblemDto>(problem);
    }

    public ProblemDto GetByIdForAdmin(long id)
    {
        var problem = _problemRepository.Get(id);
        return _mapper.Map<ProblemDto>(problem);
    }

    public ProblemDto Create(ProblemDto problemDto)
    {
        var tour = _tourService.GetById(problemDto.TourId);
        problemDto.AuthorId = tour.CreatorId;
        
        Console.WriteLine($"🔍 Creating problem for TourId={problemDto.TourId}, CreatorId={problemDto.CreatorId}, AuthorId={problemDto.AuthorId}");
        
        var problem = _mapper.Map<Problem>(problemDto);
        Console.WriteLine($"🔍 Mapped problem - AuthorId in domain: {problem.AuthorId}");
        
        var result = _problemRepository.Create(problem);
        Console.WriteLine($"🔍 Saved problem with Id={result.Id}, AuthorId={result.AuthorId}");
        
        return _mapper.Map<ProblemDto>(result);
    }

    public ProblemDto Update(ProblemDto problemDto)
    {
        var problem = _problemRepository.Get(problemDto.Id);
        var category = _mapper.Map<DomainProblemCategory>(problemDto.Category);
        problem.Update(problemDto.Priority, problemDto.Description, category);
        var result = _problemRepository.Update(problem);
        return _mapper.Map<ProblemDto>(result);
    }

    public void Delete(long id)
    {
        _problemRepository.Delete(id);
    }

    public ProblemDto ChangeProblemStatus(long problemId, long touristId, DtoProblemStatus newStatus, string? comment)
    {
        var problem = _problemRepository.Get(problemId);

        if (problem.CreatorId != touristId)
            throw new UnauthorizedAccessException("Only the tourist who reported the problem can change its status.");

        switch (newStatus)
        {
            case DtoProblemStatus.ResolvedByTourist:
                problem.MarkAsResolvedByTourist(comment);
                break;
            case DtoProblemStatus.Unresolved:
                problem.MarkAsUnresolved(comment);
                break;
            default:
                throw new ArgumentException($"Cannot change status to {newStatus}.");
        }

        var result = _problemRepository.Update(problem);
        return _mapper.Map<ProblemDto>(result);
    }

    public ProblemDto SetAdminDeadline(long problemId, DateTime deadline)
    {
        var problem = _problemRepository.Get(problemId);
        problem.SetAdminDeadline(deadline);
        var result = _problemRepository.Update(problem);
        return _mapper.Map<ProblemDto>(result);
    }
}
