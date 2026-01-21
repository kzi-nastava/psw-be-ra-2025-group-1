using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain.Bundles;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Core.Adapters;

namespace Explorer.Payments.Core.UseCases;

public class BundleService : IBundleService
{
    private readonly IBundleRepository _bundleRepository;
    private readonly ITourRepositoryAdapter _tourRepository;
    private readonly IMapper _mapper;

    public BundleService(IBundleRepository bundleRepository, ITourRepositoryAdapter tourRepository, IMapper mapper)
    {
        _bundleRepository = bundleRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public BundleDto Create(long authorId, BundleCreationDto bundleDto)
    {
        var bundle = new Bundle(authorId, bundleDto.Name, bundleDto.Price, bundleDto.TourIds);
        var result = _bundleRepository.Create(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public BundleDto Update(long authorId, long bundleId, BundleCreationDto bundleDto)
    {
        var bundle = _bundleRepository.Get(bundleId);
        if (bundle == null) throw new KeyNotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new UnauthorizedAccessException("You are not the author of this bundle.");

        bundle.Update(bundleDto.Name, bundleDto.Price, bundleDto.TourIds);
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public void Delete(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);
        if (bundle == null) throw new KeyNotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new UnauthorizedAccessException("You are not the author of this bundle.");
        if (bundle.Status == BundleStatus.Published) throw new InvalidOperationException("Published bundle cannot be deleted.");

        _bundleRepository.Delete(bundleId);
    }

    public BundleDto Publish(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);
        if (bundle == null) throw new KeyNotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new UnauthorizedAccessException("You are not the author of this bundle.");

        var publishedTourCount = 0;
        foreach (var tourId in bundle.TourIds)
        {
            if (_tourRepository.IsTourPublished(tourId))
                publishedTourCount++;
        }

        bundle.Publish(publishedTourCount);
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public BundleDto Archive(long authorId, long bundleId)
    {
        var bundle = _bundleRepository.Get(bundleId);
        if (bundle == null) throw new KeyNotFoundException("Bundle not found.");
        if (bundle.AuthorId != authorId) throw new UnauthorizedAccessException("You are not the author of this bundle.");

        bundle.Archive();
        var result = _bundleRepository.Update(bundle);
        return _mapper.Map<BundleDto>(result);
    }

    public List<BundleDto> GetByAuthorId(long authorId)
    {
        var results = _bundleRepository.GetByAuthorId(authorId);
        return _mapper.Map<List<BundleDto>>(results);
    }

    public List<BundleDto> GetAllPublished()
    {
        var results = _bundleRepository.GetAllPublished();
        return _mapper.Map<List<BundleDto>>(results);
    }

    public List<BundleDto> GetAll()
    {
        var results = _bundleRepository.GetAll();
        return _mapper.Map<List<BundleDto>>(results);
    }

    public BundleDto Get(long id)
    {
        var result = _bundleRepository.Get(id);
        if (result == null) throw new KeyNotFoundException("Bundle not found.");
        return _mapper.Map<BundleDto>(result);
    }
}
