using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

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

    public ProblemMessageDto AddMessage(long problemId, long authorId, string content, bool isAdmin = false)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.");

        var problem = _problemRepository.Get(problemId);
        if (problem == null)
            throw new NotFoundException($"Problem with id {problemId} not found.");

        if (!isAdmin && authorId != problem.CreatorId && authorId != problem.AuthorId)
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

    public ProblemMessageDto GetById(long id)
    {
        var message = _problemMessageRepository.Get(id);
        if (message == null)
            throw new NotFoundException($"Message with id {id} not found.");

        return _mapper.Map<ProblemMessageDto>(message);
    }

    public ProblemMessageDto UpdateMessage(long messageId, long authorId, string newContent, bool isAdmin = false)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Message content cannot be empty.");

        var message = _problemMessageRepository.Get(messageId);
        if (message == null)
            throw new NotFoundException($"Message with id {messageId} not found.");

        if (!isAdmin && message.AuthorId != authorId)
            throw new UnauthorizedAccessException("Only the author or admin can update this message.");

        message.UpdateContent(newContent);
        var updatedMessage = _problemMessageRepository.Update(message);

        return _mapper.Map<ProblemMessageDto>(updatedMessage);
    }

    public void DeleteMessage(long messageId, long authorId, bool isAdmin = false)
    {
        var message = _problemMessageRepository.Get(messageId);
        if (message == null)
            throw new NotFoundException($"Message with id {messageId} not found.");

        if (!isAdmin && message.AuthorId != authorId)
            throw new UnauthorizedAccessException("Only the author or admin can delete this message.");

        _problemMessageRepository.Delete(message);
    }
}
