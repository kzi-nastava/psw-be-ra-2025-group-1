using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Explorer.Blog.Core.Domain
{
    public class Comment : Entity
    {
        public DateTime CreationDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public long UserId { get; private set; }
        public string Content { get; private set; }

        public Comment(long userId, string content)
        {
            UserId = userId;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            CreationDate = DateTime.UtcNow;
        }

        public void UpdateContent(string newContent)
        {
            if (string.IsNullOrWhiteSpace(newContent))
            {
                throw new ArgumentException("Content cannot be empty.");
            }

            if ((DateTime.UtcNow - this.CreationDate).TotalMinutes > 15)
            {
                throw new InvalidOperationException("Comments can only be updated within 15 minutes of creation.");
            }

            Content = newContent;
            LastModifiedDate = DateTime.UtcNow;
        }
    }
}
