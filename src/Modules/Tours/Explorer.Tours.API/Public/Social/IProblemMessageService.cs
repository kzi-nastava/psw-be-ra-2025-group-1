using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Social;

public interface IProblemMessageService
{
    ProblemMessageDto AddMessage(long problemId, long authorId, string content, bool isAdmin = false);
    List<ProblemMessageDto> GetMessagesByProblemId(long problemId);
    ProblemMessageDto GetById(long id);
    ProblemMessageDto UpdateMessage(long messageId, long authorId, string newContent, bool isAdmin = false);
    void DeleteMessage(long messageId, long authorId, bool isAdmin = false);
}
