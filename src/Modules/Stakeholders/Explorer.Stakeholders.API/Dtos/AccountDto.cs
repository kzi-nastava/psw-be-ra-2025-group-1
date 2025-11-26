using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class AccountDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public AccountRole Role { get; set; }
        public bool IsActive { get; set; }

        // Nemamo password
    }
    public enum AccountRole
    {
        Administrator,
        Author,
        Tourist
    }
}
