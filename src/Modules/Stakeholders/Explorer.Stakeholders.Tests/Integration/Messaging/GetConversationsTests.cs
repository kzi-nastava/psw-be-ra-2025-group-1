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

        // ------------------------------------------------------------
        // 1) VALID USER: -31 → ima konverzaciju sa -32 (ID = -300)
        // ------------------------------------------------------------
        [Fact]
        public async void GetUserConversations_ValidUser_ReturnsConversations()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var userId = -31;   // messenger1

            var result = await controller.GetUserConversations(userId);

            Assert.IsType<OkObjectResult>(result.Result);
            var ok = (OkObjectResult)result.Result;
            Assert.NotNull(ok.Value);

            var conversations = ok.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.NotEmpty(conversations);

            Assert.Contains(conversations, c =>
                c.Id == -300 &&
                (c.User1Id == -32 || c.User2Id == -32));
        }

        // ------------------------------------------------------------
        // 2) USER WITH NO CONVERSATIONS: -33 → prazna lista
        // ------------------------------------------------------------
        [Fact]
        public async void GetUserConversations_UserWithNoConversations_ReturnsEmptyList()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var userId = -33;   // nomessages@test.com

            var result = await controller.GetUserConversations(userId);

            Assert.IsType<OkObjectResult>(result.Result);
            var ok = (OkObjectResult)result.Result;

            var conversations = ok.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.Empty(conversations);
        }

        // ------------------------------------------------------------
        // 3) INVALID USER: -999 → prazna lista
        // ------------------------------------------------------------
        [Fact]
        public async void GetUserConversations_InvalidUser_ReturnsEmptyList()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var userId = -999;

            var result = await controller.GetUserConversations(userId);

            Assert.IsType<OkObjectResult>(result.Result);
            var ok = (OkObjectResult)result.Result;

            var conversations = ok.Value as List<ConversationDTO>;
            Assert.NotNull(conversations);
            Assert.Empty(conversations);
        }
    }
}
