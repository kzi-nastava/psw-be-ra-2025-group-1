
using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public.Tourist;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;


namespace Explorer.Payments.Core.UseCases
{
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

            // AutoMapper već ima mapu: TourPurchaseToken <-> TourPurchaseTokenDto
            var dtos = _mapper.Map<List<TourPurchaseTokenDto>>(tokens);
            return dtos;
        }

        public bool HasValidToken(long userId, long tourId)
        {
            // Uzimamo sve token-e korisnika i proveravamo:
            // - da je za tu turu
            // - da je IsValid == true (Status == Active)
            var tokens = _tokenRepository.GetByUserId(userId);

            var hasValid = tokens.Any(t =>
                    t.TourId == tourId &&
                    t.IsValid   // IsValid je domen property (Status == Active)
            );

            return hasValid;
        }
    }
}