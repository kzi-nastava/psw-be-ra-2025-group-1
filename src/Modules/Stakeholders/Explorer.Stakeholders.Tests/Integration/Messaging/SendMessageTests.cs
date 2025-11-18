using Explorer.API.Controllers.User.Messaging;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    public class SendMessageTests : MessagesControllerTests
    {
        public SendMessageTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public async void SendMessage_Successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var request = new SendMessageRequest
            {
                SenderId = -1,
                ReceiverId = -2,
                Content = "Test poruka iz testa"
            };

            // Act
            var result = await controller.SendMessage(request);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okResult = (OkObjectResult)result.Result;
            Assert.NotNull(okResult.Value);
            Assert.IsType<MessageDTO>(okResult.Value);

            var message = (MessageDTO)okResult.Value;
            Assert.Equal(request.Content, message.Content);
            Assert.Equal(request.SenderId, message.SenderId);
            Assert.Equal(request.ReceiverId, message.ReceiverId);
            Assert.True(message.Id != 0);
        }
    }
}