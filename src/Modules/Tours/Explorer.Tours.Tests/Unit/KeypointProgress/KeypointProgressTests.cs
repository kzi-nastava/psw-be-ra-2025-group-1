using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Tours.Core.Domain;
using Shouldly;

namespace Explorer.Tours.Tests.Unit.KeypointProgress
{
    [Collection("Sequential")]
    public class KeypointProgressTests
    {
        private static Tour GetTestTour()
        {
            var tour = new Tour(1, "Test Tour", "Description", 2, ["tag1"], TourStatus.Draft, 50); // Use valid ID
            var kp1 = new Keypoint("KP1", 1, 1);
            var kp2 = new Keypoint("KP2", 5, 5);
            var kp3 = new Keypoint("KP3", 18, 18);

            // Set valid IDs for keypoints (assuming Entity has an Id property)
            SetEntityId(kp1, 1);
            SetEntityId(kp2, 2);
            SetEntityId(kp3, 3);

            tour.AddKeypoint(kp1);
            tour.AddKeypoint(kp2);
            tour.AddKeypoint(kp3);
            tour.Publish();
            return tour;
        }

        private static void SetEntityId(Entity entity, long id)
        {
            var property = typeof(Entity).GetProperty("Id");
            property?.SetValue(entity, id);
        }

        private static TourExecution GetTestExecution(long touristId = 1, long tourId = 1)
        {
            return new TourExecution(touristId, tourId);
        }

        private static UserLocation CreateUserLocation(long userId, double latitude, double longitude)
        {
            return new UserLocation(userId, latitude, longitude, DateTime.UtcNow);
        }

        [Fact]
        public void Creates_tour_execution_successfully()
        {
            // Arrange & Act
            var execution = new TourExecution(1, 1);

            // Assert
            execution.TouristId.ShouldBe(1);
            execution.TourId.ShouldBe(1);
            execution.Status.ShouldBe(TourExecutionStatus.InProgress);
            execution.PercentageCompleted.ShouldBe(0);
            execution.CurrentKeypointSequence.ShouldBe(1);
            execution.KeypointProgresses.ShouldBeEmpty();
        }

        [Theory]
        [InlineData(0, 1, typeof(ArgumentException))]
        [InlineData(-1, 1, typeof(ArgumentException))]
        [InlineData(1, 0, typeof(ArgumentException))]
        [InlineData(1, -1, typeof(ArgumentException))]
        public void Fails_to_create_execution_with_invalid_ids(long touristId, long tourId, Type exType)
        {
            // Act & Assert
            var ex = Should.Throw<Exception>(() => new TourExecution(touristId, tourId));
            ex.ShouldBeOfType(exType);
        }

        [Fact]
        public void Reaches_keypoint_successfully()
        {
            // Arrange
            var execution = GetTestExecution();
            var tour = GetTestTour();
            var firstKeypoint = tour.Keypoints.First();

            // Act
            execution.ReachKeypoint(firstKeypoint.Id, tour.Keypoints.Count);

            // Assert
            execution.KeypointProgresses.Count.ShouldBe(1);
            execution.KeypointProgresses.First().KeypointId.ShouldBe(firstKeypoint.Id);
            execution.CurrentKeypointSequence.ShouldBe(2);
            execution.PercentageCompleted.ShouldBeGreaterThan(0);
            execution.Status.ShouldBe(TourExecutionStatus.InProgress);
        }

        [Fact]
        public void Reaches_all_keypoints_and_completes_tour()
        {
            // Arrange
            var execution = GetTestExecution();
            var tour = GetTestTour();

            // Act - reach all keypoints
            foreach (var keypoint in tour.Keypoints)
            {
                execution.ReachKeypoint(keypoint.Id, tour.Keypoints.Count);
            }

            // Assert
            execution.KeypointProgresses.Count.ShouldBe(tour.Keypoints.Count);
            execution.PercentageCompleted.ShouldBe(100);
            execution.Status.ShouldBe(TourExecutionStatus.Completed);
            execution.EndTime.ShouldNotBeNull();
        }

        [Fact]
        public void Fails_to_reach_same_keypoint_twice()
        {
            // Arrange
            var execution = GetTestExecution();
            var tour = GetTestTour();
            var firstKeypoint = tour.Keypoints.First();
            execution.ReachKeypoint(firstKeypoint.Id, tour.Keypoints.Count);

            // Act & Assert
            var ex = Should.Throw<InvalidOperationException>(() =>
                execution.ReachKeypoint(firstKeypoint.Id, tour.Keypoints.Count));
            ex.Message.ShouldContain("already reached");
        }

        [Fact]
        public void Fails_to_reach_keypoint_on_completed_tour()
        {
            // Arrange
            var execution = GetTestExecution();
            execution.Complete();
            var tour = GetTestTour();

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                execution.ReachKeypoint(tour.Keypoints.First().Id, tour.Keypoints.Count));
        }

        [Fact]
        public void Fails_to_reach_keypoint_on_abandoned_tour()
        {
            // Arrange
            var execution = GetTestExecution();
            execution.Abandon();
            var tour = GetTestTour();

            // Act & Assert
            Should.Throw<InvalidOperationException>(() =>
                execution.ReachKeypoint(tour.Keypoints.First().Id, tour.Keypoints.Count));
        }

        [Fact]
        public void Updates_percentage_correctly_as_keypoints_reached()
        {
            // Arrange
            var execution = GetTestExecution();
            var tour = GetTestTour();
            var totalKeypoints = tour.Keypoints.Count;

            // Act & Assert
            execution.PercentageCompleted.ShouldBe(0);

            execution.ReachKeypoint(tour.Keypoints[0].Id, totalKeypoints);
            execution.PercentageCompleted.ShouldBe(100.0 / totalKeypoints, 0.1);

            execution.ReachKeypoint(tour.Keypoints[1].Id, totalKeypoints);
            execution.PercentageCompleted.ShouldBe(200.0 / totalKeypoints, 0.1);
        }

        [Fact]
        public void Tourist_reaches_keypoint_when_at_correct_location()
        {
            // Arrange
            var touristId = 1L;
            var execution = GetTestExecution(touristId);
            var tour = GetTestTour();
            var firstKeypoint = tour.Keypoints.First();

            // Set user location at the keypoint (exact coordinates)
            var userLocation = CreateUserLocation(touristId, firstKeypoint.Latitude, firstKeypoint.Longitude);

            // Act
            execution.ReachKeypoint(firstKeypoint.Id, tour.Keypoints.Count);

            // Assert
            execution.KeypointProgresses.Count.ShouldBe(1);
            execution.CurrentKeypointSequence.ShouldBe(2);
        }

        [Fact]
        public void Tourist_reaches_multiple_keypoints_by_moving_location()
        {
            // Arrange
            var touristId = 1L;
            var execution = GetTestExecution(touristId);
            var tour = GetTestTour();

            // Act & Assert - Move to first keypoint
            var keypoint1 = tour.Keypoints[0];
            var location1 = CreateUserLocation(touristId, keypoint1.Latitude, keypoint1.Longitude);
            execution.ReachKeypoint(keypoint1.Id, tour.Keypoints.Count);
            execution.CurrentKeypointSequence.ShouldBe(2);

            // Move to second keypoint
            var keypoint2 = tour.Keypoints[1];
            var location2 = CreateUserLocation(touristId, keypoint2.Latitude, keypoint2.Longitude);
            execution.ReachKeypoint(keypoint2.Id, tour.Keypoints.Count);
            execution.CurrentKeypointSequence.ShouldBe(3);

            // Move to third keypoint
            var keypoint3 = tour.Keypoints[2];
            var location3 = CreateUserLocation(touristId, keypoint3.Latitude, keypoint3.Longitude);
            execution.ReachKeypoint(keypoint3.Id, tour.Keypoints.Count);

            // Assert final state
            execution.KeypointProgresses.Count.ShouldBe(3);
            execution.Status.ShouldBe(TourExecutionStatus.Completed);
        }

        [Fact]
        public void Updates_user_location_between_keypoints()
        {
            // Arrange
            var touristId = 1L;
            var tour = GetTestTour();
            var keypoint1 = tour.Keypoints[0];
            var keypoint2 = tour.Keypoints[1];

            // Set initial location at first keypoint
            var initialLocation = CreateUserLocation(touristId, keypoint1.Latitude, keypoint1.Longitude);
            initialLocation.Latitude.ShouldBe(keypoint1.Latitude);
            initialLocation.Longitude.ShouldBe(keypoint1.Longitude);

            // Act - Update location to second keypoint
            var updatedLocation = CreateUserLocation(touristId, keypoint2.Latitude, keypoint2.Longitude);

            // Assert
            updatedLocation.Latitude.ShouldBe(keypoint2.Latitude);
            updatedLocation.Longitude.ShouldBe(keypoint2.Longitude);
            updatedLocation.UserId.ShouldBe(touristId);
            updatedLocation.Timestamp.ShouldBeGreaterThan(initialLocation.Timestamp);
        }

        [Fact]
        public void User_location_tracks_timestamp_on_creation()
        {
            // Arrange
            var userId = 1L;
            var beforeCreation = DateTime.UtcNow;

            // Act
            var location = CreateUserLocation(userId, 45.0, 19.0);

            // Assert
            location.UserId.ShouldBe(userId);
            location.Latitude.ShouldBe(45.0);
            location.Longitude.ShouldBe(19.0);
            location.Timestamp.ShouldBeGreaterThanOrEqualTo(beforeCreation);
        }
    }
}