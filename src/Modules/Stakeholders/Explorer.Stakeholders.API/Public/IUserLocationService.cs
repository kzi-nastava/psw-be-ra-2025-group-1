using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IUserLocationService
    {
        UserLocationDto Create(UserLocationDto userLocation);
        UserLocationDto GetByUserId(long userId);
        UserLocationDto Get(long id);
        UserLocationDto Update(UserLocationDto userLocation);
        bool Delete(long id);
    }
}
