using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.TourPurchaseTokens;

namespace Explorer.Tours.Core.UseCases
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