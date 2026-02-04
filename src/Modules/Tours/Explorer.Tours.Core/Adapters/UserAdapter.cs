using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Adapters
{
    public class UserAdapter : IUserAdapter
    {
        private readonly IUserManagementService _service;
        public UserAdapter(IUserManagementService service)
        {
            _service = service;
        }


        public AccountDto GetUserById(long id)
            => _service.GetById(id);
    }
}
