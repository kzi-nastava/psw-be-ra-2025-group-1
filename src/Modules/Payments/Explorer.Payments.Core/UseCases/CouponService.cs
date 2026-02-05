using System.Security.Cryptography;
using Explorer.Payments.API.Dtos.Coupons;
using Explorer.Payments.API.Public.Author;
using Explorer.Payments.Core.Domain.Cupons;
using Explorer.Payments.Core.Domain.External;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepo;
        private readonly ITourInfoService _tourInfo;

        public CouponService(ICouponRepository couponRepo, ITourInfoService tourInfo)
        {
            _couponRepo = couponRepo;
            _tourInfo = tourInfo;
        }

        public CouponDto Create(long authorId, CreateCouponRequestDto req)
        {
            if (authorId <= 0) throw new ArgumentException("AuthorId is required.");
            if (req.Percentage <= 0 || req.Percentage > 100m)
                throw new ArgumentException("Percentage must be in (0, 100].");

            // Ako je kupon vezan za turu: mora biti autorova tura
            if (req.TourId is not null)
            {
                var t = _tourInfo.GetById(req.TourId.Value);
                if (t.CreatorId != authorId)
                    throw new ArgumentException("You can create coupons only for your own tours.");
            }

            // 8 karaktera, UNIQUE
            string code = GenerateUniqueCode();

            var coupon = new Coupon(
                authorId: authorId,
                code: code,
                percentage: req.Percentage,
                validFromUtc: DateTime.UtcNow,
                validToUtc: req.ValidToUtc,
                tourId: req.TourId,
                isActive: true,
                maxTotalUses: req.MaxTotalUses,
                maxUsesPerUser: req.MaxUsesPerUser
            );

            var created = _couponRepo.Create(coupon);

            return new CouponDto
            {
                Id = created.Id,
                AuthorId = created.AuthorId,
                TourId = created.TourId,
                Code = created.Code,
                Percentage = created.Percentage,
                ValidFromUtc = created.ValidFromUtc,
                ValidToUtc = created.ValidToUtc,
                IsActive = created.IsActive,
                MaxTotalUses = created.MaxTotalUses,
                MaxUsesPerUser = created.MaxUsesPerUser
            };
        }

        private string GenerateUniqueCode()
        {
            // izbegni O/0, I/1 radi čitljivosti
            const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            for (int attempt = 0; attempt < 50; attempt++)
            {
                var code = GenerateCode(alphabet, 8);
                var normalized = Coupon.NormalizeCode(code);

                if (_couponRepo.GetByCode(normalized) is null)
                    return normalized;
            }

            throw new InvalidOperationException("Failed to generate unique coupon code.");
        }

        private static string GenerateCode(string alphabet, int length)
        {
            Span<byte> bytes = stackalloc byte[length];
            RandomNumberGenerator.Fill(bytes);

            var chars = new char[length];
            for (int i = 0; i < length; i++)
                chars[i] = alphabet[bytes[i] % alphabet.Length];

            return new string(chars);
        }
    }
}
