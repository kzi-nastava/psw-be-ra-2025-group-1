using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Social;

public interface IProblemMessageService
{
    ProblemMessageDto AddMessage(long problemId, long authorId, string content);
    List<ProblemMessageDto> GetMessagesByProblemId(long problemId);
}
