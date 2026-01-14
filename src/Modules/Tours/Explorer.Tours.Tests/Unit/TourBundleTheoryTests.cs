using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Explorer.Tours.Tests.Unit
{
    public class TourBundleTheoryTests
    {
        // ===================
        // 1️⃣ Kreiranje bundle
        // ===================
        [Theory]
        [InlineData(100, 200, 300)]
        [InlineData(0, 0, 0)]
        [InlineData(50, 150, 200)]
        public void CreateBundle_sets_draft_status_and_price(double tourPrice1, double tourPrice2, double bundlePrice)
        {
            // Arrange
            var authorId = 1L;

            var tours = new List<Tour>
            {
                new Tour(authorId, "Tour1", "Desc1", 5, new string[]{"tag1"}, TourStatus.Draft, tourPrice1),
                new Tour(authorId, "Tour2", "Desc2", 5, new string[]{"tag2"}, TourStatus.Draft, tourPrice2)
            };

            // Act
            var bundle = new TourBundle(authorId, "Bundle", tours, bundlePrice);

            // Assert
            bundle.Status.ShouldBe(TourBundleStatus.Draft);
            bundle.Price.ShouldBe(bundlePrice);
            bundle.Tours.Count.ShouldBe(2);
        }

        // =====================================
        // 2️⃣ Publish → zahteva ≥2 published ture
        // =====================================
        [Theory]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(1, false)]
        [InlineData(0, false)]
        public void Publish_bundle_requires_two_published_tours(int publishedToursCount, bool shouldPublish)
        {
            // Arrange
            var authorId = 1L;
            var tours = new List<Tour>();

            // Dodaj published ture
            for (int i = 0; i < publishedToursCount; i++)
                tours.Add(new Tour(authorId, $"Tour{i}", "Desc", 5, new string[] { "tag" }, TourStatus.Published, 100));

            // Dodaj jednu draft turu
            tours.Add(new Tour(authorId, "DraftTour", "Desc", 5, new string[] { "tag" }, TourStatus.Draft, 100));

            var bundle = new TourBundle(authorId, "Bundle", tours, 300);

            // Act & Assert
            if (shouldPublish)
            {
                bundle.Publish();
                bundle.Status.ShouldBe(TourBundleStatus.Published);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => bundle.Publish());
                bundle.Status.ShouldBe(TourBundleStatus.Draft);
            }
        }

        // ===============================
        // 3️⃣ Archive – samo iz Published
        // ===============================
        [Theory]
        [InlineData(TourBundleStatus.Published, true)]
        [InlineData(TourBundleStatus.Draft, false)]
        public void Archive_bundle_behavior(TourBundleStatus initialStatus, bool shouldArchive)
        {
            // Arrange
            var bundle = CreateBundleWithStatus(initialStatus);

            // Act & Assert
            if (shouldArchive)
            {
                bundle.Archive();
                bundle.Status.ShouldBe(TourBundleStatus.Archived);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => bundle.Archive());
            }
        }

        // =================================
        // 4️⃣ Activate – samo iz Archived
        // =================================
        [Theory]
        [InlineData(TourBundleStatus.Archived, true)]
        [InlineData(TourBundleStatus.Draft, false)]
        [InlineData(TourBundleStatus.Published, false)]
        public void Activate_bundle_behavior(TourBundleStatus initialStatus, bool shouldActivate)
        {
            // Arrange
            var bundle = CreateBundleWithStatus(initialStatus);

            // Act & Assert
            if (shouldActivate)
            {
                bundle.Activate();
                bundle.Status.ShouldBe(TourBundleStatus.Published);
            }
            else
            {
                Should.Throw<InvalidOperationException>(() => bundle.Activate());
            }
        }

        // ============================================
        // 5️⃣ Delete – proveri logiku preko statusa
        // ============================================
        [Theory]
        [InlineData(TourBundleStatus.Draft, true)]
        [InlineData(TourBundleStatus.Archived, true)]
        [InlineData(TourBundleStatus.Published, false)]
        public void Delete_bundle_rule(TourBundleStatus status, bool canDelete)
        {
            var bundle = CreateBundleWithStatus(status);

            // Assert – status Published = cannot delete
            (bundle.Status != TourBundleStatus.Published).ShouldBe(canDelete);
        }

        // ========================
        // Helper metode za testove
        // ========================
        private static TourBundle CreateBundleWithStatus(TourBundleStatus status)
        {
            var authorId = 1L;
            var tours = new List<Tour>
            {
                new Tour(authorId, "Tour1", "Desc", 5, new string[]{"tag"}, TourStatus.Published, 100),
                new Tour(authorId, "Tour2", "Desc", 5, new string[]{"tag"}, TourStatus.Published, 100)
            };

            var bundle = new TourBundle(authorId, "Bundle", tours, 200);

            if (status == TourBundleStatus.Published)
                bundle.Publish();
            if (status == TourBundleStatus.Archived)
            {
                bundle.Publish();
                bundle.Archive();
            }

            return bundle;
        }
    }
}
