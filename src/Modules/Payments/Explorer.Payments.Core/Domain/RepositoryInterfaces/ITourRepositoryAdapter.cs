using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces;

public interface ITourRepositoryAdapter
{
    bool IsTourPublished(long tourId);
}
