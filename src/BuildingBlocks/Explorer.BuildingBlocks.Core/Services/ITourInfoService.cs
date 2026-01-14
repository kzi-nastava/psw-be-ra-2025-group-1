using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.BuildingBlocks.Core.Services
{
    public interface ITourInfoService
    {
        TourInfoDto? GetPublishedTourInfo(long tourId);
        TourInfoDto? GetTourInfo(long tourId);
    }
}
