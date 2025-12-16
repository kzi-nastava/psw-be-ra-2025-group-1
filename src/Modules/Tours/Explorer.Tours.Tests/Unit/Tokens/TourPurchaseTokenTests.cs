using System;
using Explorer.Tours.Core.Domain.TourPurchaseTokens;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Unit.TourPurchaseTokensFolder
{
    public class TourPurchaseTokenTests
    {
        [Fact]
        public void Constructor_sets_properties_and_initial_status()
        {
            // Arrange
            var tourId = 10L;
            var userId = 5L;
            var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow);

            // Act
            var token = new TourPurchaseToken(tourId, userId, purchaseDate);

            // Assert
            token.TourId.ShouldBe(tourId);
            token.UserId.ShouldBe(userId);
            token.PurchaseDate.ShouldBe(purchaseDate);
            token.Status.ShouldBe(TourPurchaseTokenStatus.Active);
            token.IsValid.ShouldBeTrue();
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(1, 0)]
        [InlineData(1, -1)]
        public void Constructor_throws_for_invalid_ids(long tourId, long userId)
        {
            var purchaseDate = DateOnly.FromDateTime(DateTime.UtcNow);

            Should.Throw<ArgumentException>(() =>
            {
                _ = new TourPurchaseToken(tourId, userId, purchaseDate);
            });
        }


        [Fact]
        public void MarkAsUsed_sets_status_used_and_makes_token_invalid()
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            // Act
            token.MarkAsUsed();

            // Assert
            token.Status.ShouldBe(TourPurchaseTokenStatus.Used);
            token.IsValid.ShouldBeFalse();
        }

        [Theory]
        [InlineData(TourPurchaseTokenStatus.Used)]
        [InlineData(TourPurchaseTokenStatus.Expired)]
        public void MarkAsUsed_throws_if_token_not_active(TourPurchaseTokenStatus initialStatus)
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            // simulacija stanja (normalno bi ovde došli logikom u domenu)
            if (initialStatus == TourPurchaseTokenStatus.Used)
            {
                token.MarkAsUsed();
            }
            else if (initialStatus == TourPurchaseTokenStatus.Expired)
            {
                token.Expire();
            }

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
            {
                token.MarkAsUsed();
            });
        }

        [Fact]
        public void Expire_sets_status_to_expired_from_active()
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            // Act
            token.Expire();

            // Assert
            token.Status.ShouldBe(TourPurchaseTokenStatus.Expired);
            token.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Expire_sets_status_to_expired_from_used()
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            token.MarkAsUsed();

            // Act
            token.Expire();

            // Assert
            token.Status.ShouldBe(TourPurchaseTokenStatus.Expired);
            token.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Expire_is_idempotent_when_already_expired()
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            token.Expire();
            token.Status.ShouldBe(TourPurchaseTokenStatus.Expired);

            // Act
            token.Expire(); // ponovni poziv

            // Assert
            token.Status.ShouldBe(TourPurchaseTokenStatus.Expired);
        }

        [Fact]
        public void IsValid_true_only_for_active_status()
        {
            // Arrange
            var token = new TourPurchaseToken(
                tourId: 10,
                userId: 5,
                purchaseDate: DateOnly.FromDateTime(DateTime.UtcNow));

            // Act & Assert – početno ACTIVE
            token.Status.ShouldBe(TourPurchaseTokenStatus.Active);
            token.IsValid.ShouldBeTrue();

            // posle MarkAsUsed
            token.MarkAsUsed();
            token.IsValid.ShouldBeFalse();

            // posle Expire
            token.Expire();
            token.IsValid.ShouldBeFalse();
        }
    }
}
