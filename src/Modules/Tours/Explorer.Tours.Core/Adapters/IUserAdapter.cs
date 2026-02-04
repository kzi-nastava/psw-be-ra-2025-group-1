using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Adapters
{
    public interface IUserAdapter
    {
        AccountDto GetUserById(long id);
    }
}
