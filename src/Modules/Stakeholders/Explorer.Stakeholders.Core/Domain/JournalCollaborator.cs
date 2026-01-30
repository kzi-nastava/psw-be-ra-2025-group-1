using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class JournalCollaborator : Entity
    {
        public long JournalId { get; private set; }
        public long UserId { get; private set; }
        public User? User { get; private set; }


        protected JournalCollaborator() { }

        public JournalCollaborator(long journalId, long userId)
        {
            JournalId = journalId;
            UserId = userId;
        }
    }

}
