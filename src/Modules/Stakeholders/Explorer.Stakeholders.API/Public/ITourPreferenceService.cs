using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITourPreferenceService
    {
        public TourPreferenceDto Create(TourPreferenceDto tourPreference);
        public TourPreferenceDto Update(TourPreferenceDto tourPreference);
        public TourPreferenceDto Get(long id);
        public TourPreferenceDto GetByUser(long userId);
        //public TourPreferenceDto GetByPersonId(long personId);
    }
}
