using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public;

public interface IBundleService
{
    BundleDto Create(long authorId, BundleCreationDto bundle);
    BundleDto Update(long authorId, long bundleId, BundleCreationDto bundle);
    void Delete(long authorId, long bundleId);
    BundleDto Publish(long authorId, long bundleId);
    BundleDto Archive(long authorId, long bundleId);
    List<BundleDto> GetByAuthorId(long authorId);
    List<BundleDto> GetAllPublished();
    BundleDto Get(long id);
}
