using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IPersonService
    {
        PersonDto Update(PersonDto person);
        PersonDto Get(long id);
    }
}
