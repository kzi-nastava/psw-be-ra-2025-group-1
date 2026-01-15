using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class TourPurchaseTokenService : ITourPurchaseTokenService
{
    private readonly ITourPurchaseTokenRepository _tokenRepository;
    private readonly IMapper _mapper;

    public TourPurchaseTokenService(
        ITourPurchaseTokenRepository tokenRepository,
        IMapper mapper)
    {
        _tokenRepository = tokenRepository;
        _mapper = mapper;
    }

    public List<TourPurchaseTokenDto> GetByUser(long userId)
    {
        var tokens = _tokenRepository.GetByUserId(userId);
        var dtos = _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
        return dtos;
    }

    public bool HasValidToken(long userId, long tourId)
    {
        var tokens = _tokenRepository.GetByUserId(userId);
        var hasValid = tokens.Any(t =>
                t.TourId == tourId &&
                t.IsValid
        );

        return hasValid;
    }
}
