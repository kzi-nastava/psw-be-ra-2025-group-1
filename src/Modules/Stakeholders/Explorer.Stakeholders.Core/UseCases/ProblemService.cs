using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using DomainProblemStatus = Explorer.Stakeholders.Core.Domain.ProblemStatus;
using DtoProblemStatus = Explorer.Stakeholders.API.Dtos.ProblemStatus;
using DomainProblemCategory = Explorer.Stakeholders.Core.Domain.ProblemCategory;

namespace Explorer.Stakeholders.Core.UseCases;

public class ProblemService : IProblemService
{
    private readonly IProblemRepository _problemRepository;
    private readonly IMapper _mapper;

    public ProblemService(IProblemRepository repository, IMapper mapper)
    {
        _problemRepository = repository;
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
        var result = _problemRepository.GetByAuthorId(authorId, page, pageSize);
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

    public ProblemDto Create(ProblemDto problemDto)
    {
        var problem = _mapper.Map<Problem>(problemDto);
        var result = _problemRepository.Create(problem);
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
