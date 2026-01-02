using System;
using Explorer.Payments.Core.Domain.Cupons;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Unit.Coupons
{
    public class CouponTests
    {
        private static DateTime NowUtc => new DateTime(2026, 1, 2, 12, 0, 0, DateTimeKind.Utc);

        private static Coupon CreateCoupon(
            string code = " YES ",
            CouponDiscountType type = CouponDiscountType.Percentage,
            decimal amount = 10m,
            DateTime? fromUtc = null,
            DateTime? toUtc = null,
            bool isActive = true,
            int? maxTotalUses = null,
            int? maxUsesPerUser = null)
        {
            return new Coupon(
                code,
                type,
                amount,
                fromUtc ?? NowUtc.AddDays(-1),
                toUtc ?? NowUtc.AddDays(1),
                isActive,
                maxTotalUses,
                maxUsesPerUser);
        }

        [Fact]
        public void NormalizeCode_should_trim_and_uppercase()
        {
            Coupon.NormalizeCode("  aBc-12  ").ShouldBe("ABC-12");
            Coupon.NormalizeCode(null!).ShouldBe(string.Empty);
        }

        [Fact]
        public void Ctor_should_normalize_code()
        {
            var c = CreateCoupon(code: "  yes  ");
            c.Code.ShouldBe("YES");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Ctor_should_throw_when_code_empty(string code)
        {
            Should.Throw<ArgumentException>(() => CreateCoupon(code: code));
        }

        [Fact]
        public void Ctor_should_throw_when_valid_to_before_valid_from()
        {
            var from = NowUtc;
            var to = NowUtc.AddMinutes(-1);

            Should.Throw<ArgumentException>(() =>
                CreateCoupon(fromUtc: from, toUtc: to));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Ctor_should_throw_when_amount_not_positive(decimal amount)
        {
            Should.Throw<ArgumentException>(() => CreateCoupon(amount: amount));
        }

        [Fact]
        public void Ctor_should_throw_when_percentage_amount_above_100()
        {
            Should.Throw<ArgumentException>(() =>
                CreateCoupon(type: CouponDiscountType.Percentage, amount: 101m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Ctor_should_throw_when_max_total_uses_invalid(int value)
        {
            Should.Throw<ArgumentException>(() => CreateCoupon(maxTotalUses: value));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-3)]
        public void Ctor_should_throw_when_max_uses_per_user_invalid(int value)
        {
            Should.Throw<ArgumentException>(() => CreateCoupon(maxUsesPerUser: value));
        }

        [Fact]
        public void IsValidAt_should_be_true_only_if_active_and_in_window()
        {
            var c = CreateCoupon(fromUtc: NowUtc.AddHours(-1), toUtc: NowUtc.AddHours(1), isActive: true);
            c.IsValidAt(NowUtc).ShouldBeTrue();

            c.IsValidAt(NowUtc.AddHours(-2)).ShouldBeFalse();
            c.IsValidAt(NowUtc.AddHours(2)).ShouldBeFalse();

            var inactive = CreateCoupon(isActive: false);
            inactive.IsValidAt(NowUtc).ShouldBeFalse();
        }

        [Theory]
        [InlineData(30, 10, 3)]      // 10% od 30
        [InlineData(15, 10, 1.5)]    // 10% od 15
        [InlineData(50, 100, 50)]    // 100% => plafon orderTotal
        public void CalculateDiscount_percentage(decimal total, decimal percent, decimal expectedDiscount)
        {
            var c = CreateCoupon(type: CouponDiscountType.Percentage, amount: percent);

            var discount = c.CalculateDiscount(total);

            discount.ShouldBe(expectedDiscount);
        }

        [Theory]
        [InlineData(30, 5, 5)]      // fiksno 5
        [InlineData(30, 50, 30)]    // plafon na orderTotal
        public void CalculateDiscount_fixed_amount(decimal total, decimal amount, decimal expectedDiscount)
        {
            var c = CreateCoupon(type: CouponDiscountType.FixedAmount, amount: amount);

            var discount = c.CalculateDiscount(total);

            discount.ShouldBe(expectedDiscount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CalculateDiscount_should_be_zero_when_order_total_not_positive(decimal total)
        {
            var c1 = CreateCoupon(type: CouponDiscountType.Percentage, amount: 10m);
            var c2 = CreateCoupon(type: CouponDiscountType.FixedAmount, amount: 10m);

            c1.CalculateDiscount(total).ShouldBe(0);
            c2.CalculateDiscount(total).ShouldBe(0);
        }

        [Fact]
        public void Deactivate_should_make_coupon_invalid()
        {
            var c = CreateCoupon(isActive: true);
            c.IsValidAt(NowUtc).ShouldBeTrue();

            c.Deactivate();

            c.IsValidAt(NowUtc).ShouldBeFalse();
        }
    }
}
