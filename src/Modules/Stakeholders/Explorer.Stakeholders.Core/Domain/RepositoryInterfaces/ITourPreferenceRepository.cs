using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITourPreferenceRepository
    {
        TourPreference Create(TourPreference tourPreference);
        TourPreference Update(TourPreference tourPreference);
        TourPreference? Get(long id);
        TourPreference? GetByUser(long userId);
        //TourPreference GetByPersonId(long personId);
    }
}
