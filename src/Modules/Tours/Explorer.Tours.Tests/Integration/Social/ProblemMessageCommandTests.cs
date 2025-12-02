using Explorer.API.Controllers.Author;
using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Social;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Explorer.Tours.Tests.Integration.Social;

[Collection("Sequential")]
public class ProblemMessageCommandTests : BaseToursIntegrationTest
{
    public ProblemMessageCommandTests(ToursTestFactory factory) : base(factory) { }

    [Fact]
    public void Tourist_adds_message_to_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, "-21");
        var messageDto = new AddProblemMessageDto
        {
            ProblemId = -1,
            Content = "Additional details about the problem"
        };

        // Act
        var result = ((ObjectResult)controller.AddMessage(messageDto).Result)?.Value as ProblemMessageDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.ProblemId.ShouldBe(-1);
        result.AuthorId.ShouldBe(-21);
        result.Content.ShouldBe("Additional details about the problem");
        result.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public void Author_adds_message_to_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateAuthorController(scope, "-11");
        var messageDto = new AddProblemMessageDto
        {
            ProblemId = -1,
            Content = "We are working on resolving this issue"
        };

        // Act
        var result = ((ObjectResult)controller.AddMessage(messageDto).Result)?.Value as ProblemMessageDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.ProblemId.ShouldBe(-1);
        result.AuthorId.ShouldBe(-11);
        result.Content.ShouldBe("We are working on resolving this issue");
    }

    [Fact]
    public void Add_message_fails_when_user_not_participant()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, "-23");
        var messageDto = new AddProblemMessageDto
        {
            ProblemId = -1,
            Content = "I should not be able to send this"
        };

        // Act
        var result = controller.AddMessage(messageDto).Result;

        // Assert
        result.ShouldBeOfType<ForbidResult>();
    }

    [Fact]
    public void Add_message_fails_with_empty_content()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, "-21");
        var messageDto = new AddProblemMessageDto
        {
            ProblemId = -1,
            Content = ""
        };

        // Act
        var result = controller.AddMessage(messageDto).Result;

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Add_message_fails_for_nonexistent_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, "-21");
        var messageDto = new AddProblemMessageDto
        {
            ProblemId = -999,
            Content = "Message to nonexistent problem"
        };

        // Act
        var result = controller.AddMessage(messageDto).Result;

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Get_messages_for_problem()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateTouristController(scope, "-21");

        // Act
        var actionResult = controller.GetMessages(-1).Result;

        // Assert
        actionResult.ShouldNotBeNull();
        actionResult.ShouldBeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)actionResult;
        var result = okResult.Value as List<ProblemMessageDto>;

        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldAllBe(m => m.ProblemId == -1);
    }

    private static TouristProblemMessageController CreateTouristController(IServiceScope scope, string personId)
    {
        return new TouristProblemMessageController(
            scope.ServiceProvider.GetRequiredService<IProblemMessageService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.INotificationService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.IProblemService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }

    private static AuthorProblemMessageController CreateAuthorController(IServiceScope scope, string personId)
    {
        return new AuthorProblemMessageController(
            scope.ServiceProvider.GetRequiredService<IProblemMessageService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.INotificationService>(),
            scope.ServiceProvider.GetRequiredService<Explorer.Stakeholders.API.Public.IProblemService>())
        {
            ControllerContext = BuildContext(personId)
        };
    }
}
