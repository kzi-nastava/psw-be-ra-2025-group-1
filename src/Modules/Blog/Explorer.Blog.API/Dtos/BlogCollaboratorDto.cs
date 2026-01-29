using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogCollaboratorDto
    {
        public long UserId { get; set; }
        public string Username { get; set; } = "";
    }
}
