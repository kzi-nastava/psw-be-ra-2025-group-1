using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

public class Vote : Entity

{
    public long BlogId { get; private set; }
    public long UserId { get; private set; }
    
    public VoteType VoteType { get; private set; }

    public DateTime CreationTime { get; private set; }

    public Vote(long blogId, long userId, VoteType voteType) 
    {
        BlogId = blogId;
        UserId = userId;
        VoteType = voteType;
        CreationTime = DateTime.UtcNow; 
    }

    private Vote() {}
}