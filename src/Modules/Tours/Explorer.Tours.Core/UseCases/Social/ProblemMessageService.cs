using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Social;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Social;

public class ProblemMessageService : IProblemMessageService
{
    private readonly IProblemMessageRepository _problemMessageRepository;
    private readonly IProblemRepository _problemRepository;
    private readonly IMapper _mapper;

    public ProblemMessageService(
        IProblemMessageRepository problemMessageRepository,
        IProblemRepository problemRepository,
        IMapper mapper)
    {
        _problemMessageRepository = problemMessageRepository;
        _problemRepository = problemRepository;
        _mapper = mapper;
    }

    public ProblemMessageDto AddMessage(long problemId, long authorId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.");

        var problem = _problemRepository.Get(problemId);
        if (problem == null)
            throw new NotFoundException($"Problem with id {problemId} not found.");

        if (authorId != problem.CreatorId && authorId != problem.AuthorId)
            throw new UnauthorizedAccessException("Only participants can add messages to this problem.");

        var message = new ProblemMessage(problemId, authorId, content);
        var savedMessage = _problemMessageRepository.Add(message);

        return _mapper.Map<ProblemMessageDto>(savedMessage);
    }

    public List<ProblemMessageDto> GetMessagesByProblemId(long problemId)
    {
        var messages = _problemMessageRepository.GetByProblemId(problemId);
        return messages.Select(_mapper.Map<ProblemMessageDto>).ToList();
    }
}
