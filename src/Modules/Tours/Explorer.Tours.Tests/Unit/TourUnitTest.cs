using Explorer.Tours.Core.Domain;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Unit
{
    public enum ExceptionType
    {
        None,
        InvalidOperationException,
        ArgumentException
    }

    [Collection("Sequential")]
    public class TourUnitTest
    {
        private static Tour GetTestTour(string type)
        {
            Dictionary<string, Tour> tours = new Dictionary<string, Tour>();
            tours.Add("draft", new Tour(-1, "Draft tour", "Desc", 2, ["tag1", "tag2"], TourStatus.Draft, 50));
            tours.Add("publish", new Tour(-1, "Draft tour", "Desc", 2, ["tag1", "tag2"], TourStatus.Published, 50));

            return tours[type];
        }



        [Theory]
        [InlineData("draft", "KP1", 0, 0, null)] // Valid, adds to tour in draft
        [InlineData("publish", "KP1", 0, 0, typeof(InvalidOperationException))] // Invalid, adds to tour not in draft
        // Ovi testovi vrv trebaju da idu u KeypointUnitTest v
        [InlineData("draft", "", 0, 0, typeof(ArgumentException))] // Invalid, no title provided
        [InlineData("draft", "KP1", 200, 0, typeof(ArgumentException))] // Invalid, invalid latitude
        [InlineData("draft", "KP1", 0, 200, typeof(ArgumentException))] // Invalid, invalid longitude
        public void Adds_keypoint(string tourType,  string keypointTitle, double latitude, double longitude, Type exType)
        {
            // Arrange
            Tour tour = GetTestTour(tourType);

            if (exType == null)
            {
                Keypoint kp = new Keypoint(keypointTitle, latitude, longitude);

                // Act
                tour.AddKeypoint(kp);
                
                // Assert
                tour.Keypoints.ShouldContain(kp);
            }
            else if (exType == typeof(InvalidOperationException))
            {
                Keypoint kp = new Keypoint(keypointTitle, latitude, longitude);

                // Act
                var ex = Should.Throw<Exception>(() => tour.AddKeypoint(kp));

                ex.ShouldBeOfType(exType);
            }
            else
            {
                var ex = Should.Throw<Exception>(() => new Keypoint(keypointTitle, latitude, longitude));
                ex.ShouldBeOfType(exType);
            }

        }

       // More tests.. 🙃  🦆🥟
    }
}
