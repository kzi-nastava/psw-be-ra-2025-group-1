using Explorer.API.Controllers.User.Messaging;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Tests.Integration.Messaging;
using Explorer.Stakeholders.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Explorer.Stakeholders.Tests.Integration.Messaging // DODAJ OVO
{
    public class DeleteMessageTests : MessagesControllerTests
    {
        public DeleteMessageTests(StakeholdersTestFactory factory) : base(factory) { }

        [Fact]
        public async void DeleteMessage_Successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Prvo pošalji novu poruku da bi je obrisao
            var sendRequest = new SendMessageRequest
            {
                SenderId = -31,
                ReceiverId = -32,
                Content = "Poruka za brisanje"
            };

            var sendResult = await controller.SendMessage(sendRequest);
            var sentMessage = (MessageDTO)((OkObjectResult)sendResult.Result).Value;
            var messageIdToDelete = sentMessage.Id;

            // Act
            var result = await controller.DeleteMessage(messageIdToDelete);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async void DeleteMessage_NonExistentMessage_ReturnsNotFound()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var messageId = -999; // Poruka koja sigurno ne postoji

            // Act
            var result = await controller.DeleteMessage(messageId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}