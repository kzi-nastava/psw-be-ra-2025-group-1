using Explorer.API.Controllers;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Explorer.Blog.Tests.Integration
{
    public class BlogControllerCommentsTests : BaseBlogIntegrationTest
    {
        public BlogControllerCommentsTests(BlogTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void AddComment()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var newEntity = new CommentCreateDto
            {
                Content = "This is a test comment."
            };

            var result = ((ObjectResult)controller.AddComment(-2L, newEntity).Result)?.Value as CommentDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Content.ShouldBe(newEntity.Content);

            // Assert - Database
            var storedEntity = dbContext.Comments.FirstOrDefault(i => i.Content == newEntity.Content);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void AddComment_fails_draft()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var newEntity = new CommentCreateDto
            {
                Content = "This is a test comment."
            };

            Should.Throw<InvalidOperationException>(() => controller.AddComment(-1L, newEntity));
        }

        [Fact]
        public void AddComment_fails_archived()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();

            var newEntity = new CommentCreateDto
            {
                Content = "This is a test comment."
            };

            Should.Throw<InvalidOperationException>(() => controller.AddComment(-3L, newEntity));
        }

        [Fact]
        public void Update()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
            var updatedEntity = new CommentUpdateDto
            {
                Id = -1,
                Content = "This is an updated test comment."
            };
            var result = ((ObjectResult)controller.UpdateComment(-2, -1, updatedEntity).Result)?.Value as CommentDto;
            result.ShouldNotBeNull();
            result.Id.ShouldBe(updatedEntity.Id);
            result.Content.ShouldBe(updatedEntity.Content);
            // Assert - Database
            var storedEntity = dbContext.Comments.FirstOrDefault(i => i.Id == updatedEntity.Id);
            storedEntity.ShouldNotBeNull();
            storedEntity.Content.ShouldBe(updatedEntity.Content);
        }

        [Fact]
        public void Update_fails_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
            var updatedEntity = new CommentUpdateDto
            {
                Id = 9999,
                Content = "This is an updated test comment."
            };
            Should.Throw<KeyNotFoundException>(() => controller.UpdateComment(-2, 9999, updatedEntity));
        }

        [Fact]
        public void Delete()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
            // First, create a comment to delete
            var testEntity = new CommentCreateDto
            {
                Content = "Comment to be deleted."
            };
            var createdComment = ((ObjectResult)controller.AddComment(-2L, testEntity).Result)?.Value as CommentDto;
            // Act - Delete
            var result = (OkResult)controller.DeleteComment(-2, createdComment.Id);
            // Assert - Response
            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);
            // Assert - Database
            var storedEntity = dbContext.Comments.FirstOrDefault(i => i.Id == createdComment.Id);
            storedEntity.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<BlogContext>();
            Should.Throw<KeyNotFoundException>(() => controller.DeleteComment(-2, 9999));
        }

        private static BlogController CreateController(IServiceScope scope)
        {
            return new BlogController(scope.ServiceProvider.GetRequiredService<IBlogService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
