using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IUserLocationRepository
    {
        UserLocation Create(UserLocation entity);
        UserLocation? GetByUserId(long userId);
        UserLocation? Get(long id);
        UserLocation Update(UserLocation entity);
        bool Delete(long id);
    }
}
