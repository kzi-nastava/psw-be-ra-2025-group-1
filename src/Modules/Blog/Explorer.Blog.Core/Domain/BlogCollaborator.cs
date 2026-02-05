using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Blog.Core.Domain
{
    public class BlogCollaborator : Entity
    {
        public long BlogId { get; private set; }
        public long UserId { get; private set; }


        protected BlogCollaborator() { }

        public BlogCollaborator(long blogId, long userId)
        {
            BlogId = blogId;
            UserId = userId;
        }
    }
}
