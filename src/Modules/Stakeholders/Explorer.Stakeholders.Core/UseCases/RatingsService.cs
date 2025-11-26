using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.UseCases;

public class RatingsService : IRatingsService
{
    private readonly IRatingRepository _repo;
    private readonly IMapper _mapper;

    public RatingsService(IRatingRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public RatingDto Create(long userId, RatingCreateDto dto)
    {
        // Ako dozvoljavate samo jednu ocenu po korisniku:
        var existing = _repo.GetSingleByUserId(userId);
        if (existing != null)
            throw new EntityValidationException("Korisnik već ima ocenu. Izmenite postojeću ocenu.");

        var rating = new Rating(userId, dto.Score, dto.Comment);
        rating = _repo.Add(rating);
        return _mapper.Map<RatingDto>(rating);
    }

    public List<RatingDto> GetMine(long userId)
    {
        var list = _repo.GetByUserId(userId).ToList();
        return _mapper.Map<List<RatingDto>>(list);
    }

    public RatingDto Update(long userId, long ratingId, RatingUpdateDto dto)
    {
        var rating = _repo.GetById(ratingId) ?? throw new KeyNotFoundException("Ocena nije pronađena.");
        if (rating.UserId != userId) throw new UnauthorizedAccessException("Nemate dozvolu da menjate ovu ocenu.");

        rating.Update(dto.Score, dto.Comment);
        _repo.Update(rating);
        return _mapper.Map<RatingDto>(rating);
    }

    public void Delete(long userId, long ratingId)
    {
        var rating = _repo.GetById(ratingId) ?? throw new KeyNotFoundException("Ocena nije pronađena.");
        if (rating.UserId != userId) throw new UnauthorizedAccessException("Nemate dozvolu da brišete ovu ocenu.");

        _repo.Delete(rating);
    }
    public PagedResult<RatingDto> AdminList(int page, int size)
    {
        if (page <= 0) page = 1;
        if (size <= 0) size = 10;

        var (items, total) = _repo.GetPaged(page, size);

        // BuildingBlocks PagedResult očekuje List<T> u konstruktoru
        var mapped = _mapper.Map<List<RatingDto>>(items);

        return new PagedResult<RatingDto>(mapped, total);
    }

}
