using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristMapMarkerCommandTests : BaseToursIntegrationTest
    {
        public TouristMapMarkerCommandTests(ToursTestFactory factory) : base(factory) { }

        private static TouristMapMarkerController CreateController(IServiceScope scope, string userId = "-1")
        {
            var controller = new TouristMapMarkerController(scope.ServiceProvider.GetRequiredService<ITouristMapMarkerService>());

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", userId),
                    new Claim("id", userId),
                    new Claim(ClaimTypes.Role, "tourist")
                }, "test"))
                }
            };

            return controller;

        }

        [Fact]
        public void CanGetPagedMarkers()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = controller.GetPagedByTourist(-22, 0, 10);
            result.Result.ShouldBeOfType<OkObjectResult>();

            var ok = (OkObjectResult)result.Result;
            ok.StatusCode.ShouldBe(200);

            var data = ok.Value as PagedResult<TouristMapMarkerDto>;
            data.ShouldNotBeNull();
            data.Results.ShouldNotBeEmpty();
        }

        [Fact]
        public void CanGetAllMarkers()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = controller.GetAllByTourist(-22);
            result.Result.ShouldBeOfType<OkObjectResult>();

            var ok = (OkObjectResult)result.Result;
            ok.StatusCode.ShouldBe(200);

            var markers = ok.Value as List<TouristMapMarkerDto>;
            markers.ShouldNotBeNull();
            markers.ShouldContain(m => m.TouristId == -22);
        }

        [Fact]
        public void CanGetActiveMarker()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = controller.GetActive(-22);
            result.Result.ShouldBeOfType<OkObjectResult>();

            var ok = (OkObjectResult)result.Result;
            ok.StatusCode.ShouldBe(200);

            var marker = ok.Value as TouristMapMarkerDto;
            marker.ShouldNotBeNull();
            marker.IsActive.ShouldBeTrue();
        }

        [Fact]
        public void CanSetMarkerActive()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var markerId = -81010;

            var result = controller.SetActive(markerId);
            result.Result.ShouldBeOfType<OkObjectResult>();

            var ok = (OkObjectResult)result.Result;
            ok.StatusCode.ShouldBe(200);

            var dto = ok.Value as TouristMapMarkerDto;
            dto.ShouldNotBeNull();
            dto.IsActive.ShouldBeTrue();

            // Ensure only one active marker
            var allMarkers = db.TouristMapMarkers.Where(tm => tm.TouristId == -22).ToList();
            allMarkers.Count(tm => tm.IsActive).ShouldBe(1);
        }

        [Fact]
        public void ActivateUncollectedMarkerFails()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, "-22");
            var db = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // marker that tourist doesn't own
            var markerId = -81013;

            var result = controller.SetActive(markerId);
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }


    }
}
