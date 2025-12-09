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
        problem.ResolvedAt.ShouldBeNull();
        problem.AdminDeadline.ShouldBeNull();
        problem.TouristComment.ShouldBeNull();
    }

    [Fact]
    public void Updates_problem_properties()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.Update(5, "Updated description", ProblemCategory.Maintenance);

        // Assert
        problem.Priority.ShouldBe(5);
        problem.Description.ShouldBe("Updated description");
        problem.Category.ShouldBe(ProblemCategory.Maintenance);
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
    public void Marks_problem_as_resolved_without_comment()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.MarkAsResolvedByTourist();

        // Assert
        problem.Status.ShouldBe(ProblemStatus.ResolvedByTourist);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBeNull();
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

    [Fact]
    public void Marks_problem_as_unresolved_without_comment()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.MarkAsUnresolved();

        // Assert
        problem.Status.ShouldBe(ProblemStatus.Unresolved);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBeNull();
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
    public void Admin_can_close_problem_with_comment()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.CloseByAdmin("Closed by administrator due to policy violation");

        // Assert
        problem.Status.ShouldBe(ProblemStatus.Unresolved);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBe("Closed by administrator due to policy violation");
    }

    [Fact]
    public void Admin_can_close_problem_without_comment()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        problem.CloseByAdmin();

        // Assert
        problem.Status.ShouldBe(ProblemStatus.Unresolved);
        problem.ResolvedAt.ShouldNotBeNull();
        problem.TouristComment.ShouldBe("Closed by administrator");
    }

    [Fact]
    public void Admin_can_close_already_resolved_problem()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        problem.MarkAsResolvedByTourist("Tourist thinks it's fixed");

        // Act
        problem.CloseByAdmin("Admin override");

        // Assert
        problem.Status.ShouldBe(ProblemStatus.Unresolved);
        problem.TouristComment.ShouldBe("Admin override");
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
    public void Set_deadline_fails_with_current_time()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var now = DateTime.UtcNow;

        // Act & Assert
        Should.Throw<ArgumentException>(() => problem.SetAdminDeadline(now));
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
    public void Does_not_detect_overdue_for_recent_problem()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);

        // Act
        var isOverdue = problem.IsOverdue();

        // Assert
        isOverdue.ShouldBeFalse();
    }

    [Fact]
    public void Overdue_only_applies_to_open_problems()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var creationTimeField = typeof(Problem).GetField("<CreationTime>k__BackingField", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        creationTimeField?.SetValue(problem, DateTime.UtcNow.AddDays(-6));
        problem.MarkAsResolvedByTourist();

        // Act
        var isOverdue = problem.IsOverdue();

        // Assert
        isOverdue.ShouldBeFalse();
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

    [Fact]
    public void Does_not_detect_missed_deadline_when_deadline_is_future()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var futureDeadline = DateTime.UtcNow.AddDays(7);
        problem.SetAdminDeadline(futureDeadline);

        // Act
        var hasMissed = problem.HasMissedAdminDeadline();

        // Assert
        hasMissed.ShouldBeFalse();
    }

    [Fact]
    public void Missed_deadline_only_applies_to_open_problems()
    {
        // Arrange
        var problem = new Problem(3, "Test problem", ProblemCategory.Safety, -1, -21, -11);
        var pastDeadline = DateTime.UtcNow.AddDays(-1);
        
        var deadlineField = typeof(Problem).GetField("<AdminDeadline>k__BackingField", 
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        deadlineField?.SetValue(problem, (DateTime?)pastDeadline);
        
        problem.MarkAsResolvedByTourist();

        // Act
        var hasMissed = problem.HasMissedAdminDeadline();

        // Assert
        hasMissed.ShouldBeFalse();
    }
}
