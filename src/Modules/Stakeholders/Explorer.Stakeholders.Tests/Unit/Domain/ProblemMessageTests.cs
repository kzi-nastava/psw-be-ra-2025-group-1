using Explorer.Stakeholders.Core.Domain;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit.Domain;

public class ProblemMessageTests
{
    [Fact]
    public void Creates_message_with_valid_data()
    {
        // Arrange & Act
        var message = new ProblemMessage(-100, -21, "Test message content");

        // Assert
        message.ProblemId.ShouldBe(-100);
        message.AuthorId.ShouldBe(-21);
        message.Content.ShouldBe("Test message content");
        message.CreatedAt.ShouldNotBe(default);
    }

    [Theory]
    [InlineData(0)]
    public void Create_fails_with_invalid_problem_id(long problemId)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new ProblemMessage(problemId, -21, "Content"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_fails_with_invalid_author_id(long authorId)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new ProblemMessage(-100, authorId, "Content"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_fails_with_empty_content(string content)
    {
        // Act & Assert
        Should.Throw<ArgumentException>(() => new ProblemMessage(-100, -21, content));
    }

    [Fact]
    public void Message_is_immutable()
    {
        // Arrange
        var message = new ProblemMessage(-100, -21, "Original content");

        // Assert 
        message.ProblemId.ShouldBe(-100);
        message.AuthorId.ShouldBe(-21);
        message.Content.ShouldBe("Original content");
        
    }

    [Fact]
    public void Creates_message_with_long_content()
    {
        // Arrange
        var longContent = new string('x', 1999); 

        // Act
        var message = new ProblemMessage(-100, -21, longContent);

        // Assert
        message.Content.Length.ShouldBe(1999);
    }
}
