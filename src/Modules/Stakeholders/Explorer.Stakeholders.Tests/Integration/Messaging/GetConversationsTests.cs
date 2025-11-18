using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    public class GetConversationsTests : MessagesControllerTests
    {
        public GetConversationsTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public async void GetUserConversations_ValidUser_ReturnsConversations()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var userId = -1;

            // Act
            var result = await controller.GetUserConversations(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.NotNull(okResult.Value);

            var conversations = okResult.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.NotEmpty(conversations);

            // Proveri da li konverzacija sa user -2 postoji
            var conversationWithUser2 = conversations.FirstOrDefault(c =>
                c.User1Id == -2 || c.User2Id == -2);
            Assert.NotNull(conversationWithUser2);
        }

        [Fact]
        public async void GetUserConversations_UserWithNoConversations_ReturnsEmptyList()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var userId = -21; // turista1 koji nema konverzacija u test podacima

            // Act
            var result = await controller.GetUserConversations(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.NotNull(okResult.Value);

            var conversations = okResult.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.Empty(conversations);
        }

        [Fact]
        public async void GetUserConversations_InvalidUser_ReturnsEmptyList()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var userId = -999; // Ne postojići user

            // Act
            var result = await controller.GetUserConversations(userId);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.NotNull(okResult.Value);

            var conversations = okResult.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.Empty(conversations);
        }
    }
}