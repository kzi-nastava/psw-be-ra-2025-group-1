using Explorer.Stakeholders.Core.Domain;
using Shouldly;

namespace Explorer.Stakeholders.Tests.Unit.Domain;

public class ProblemTests
{
    [Fact]
    public void Creates_problem_with_valid_data()
    {
        // Arrange & Act
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Assert
        problem.Priority.ShouldBe(3);
        problem.Description.ShouldBe("Test problem");
        problem.Category.ShouldBe(ProblemCategory.Safety);
        problem.TourId.ShouldBe(-1);
        problem.CreatorId.ShouldBe(-21);
        problem.AuthorId.ShouldBe(-11);
        problem.Status.ShouldBe(ProblemStatus.Open);
        problem.CreationTime.ShouldNotBe(default);
    }

    [Fact]
    public void Marks_problem_as_resolved_by_tourist()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.MarkAsResolvedByTourist("Fixed successfully");

        // Assert
        problem.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBe("Fixed successfully");
    }

    [Fact]
    public void Marks_problem_as_unresolved()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.MarkAsUnresolved("Still an issue");

        // Assert
        problem.Status.ShouldBe(ProblemStatus.Unresolved);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBe("Still an issue");
    }

    [Theory]
    [InlineData(ProblemStatus.ResolvedByTourist)]
    [InlineData(ProblemStatus.Unresolved)]
    public void Cannot_change_status_of_non_open_problem(ProblemStatus currentStatus)
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        if (currentStatus == ProblemStatus.ResolvedByTourist)
            problem.MarkAsResolvedByTourist();
        else
            problem.MarkAsUnresolved();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() => problem.MarkAsResolvedByTourist("Trying again"));
        Should.Throw<InvalidOperationException>(() => problem.MarkAsUnresolved("Trying again"));
    }

    [Fact]
    public void Sets_admin_deadline()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var deadline = DateTime.UtcNow.AddDays(7);

        // Act
        problem.SetAdminDeadline(deadline);

        // Assert
        problem.AdminDeadline.ShouldNotBeNull();
        problem.AdminDeadline.Value.Date.ShouldBe(deadline.Date);
    }

    [Fact]
    public void Set_deadline_fails_with_past_date()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act & Assert
        Should.Throw<ArgumentException>(() => problem.SetAdminDeadline(pastDate));
    }

    [Fact]
    public void Detects_overdue_problem()
    {
        // Arrange 
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var creationTimeField = typeof(Problem).GetField("<CreationTime>k__BackingField", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        creationTimeField?.SetValue(problem, DateTime.UtcNow.AddDays(-6));

        // Act
        var isOverdue = problem.IsOverdue();

        // Assert
        isOverdue.ShouldBeTrue();
    }

    [Fact]
    public void Detects_missed_admin_deadline()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var pastDeadline = DateTime.UtcNow.AddDays(-1);
        
        // Use reflection to set the deadline directly
        var deadlineField = typeof(Problem).GetField("<AdminDeadline>k__BackingField", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        deadlineField?.SetValue(problem, (DateTime?)pastDeadline);

        // Act
        var hasMissed = problem.HasMissedAdminDeadline();

        // Assert
        hasMissed.ShouldBeTrue();
    }

    [Fact]
    public void Does_not_detect_missed_deadline_when_no_deadline_set()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        var hasMissed = problem.HasMissedAdminDeadline();

        // Assert
        hasMissed.ShouldBeFalse();
    }
}
