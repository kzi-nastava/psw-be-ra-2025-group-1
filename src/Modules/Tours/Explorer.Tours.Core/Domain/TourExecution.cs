using Explorer.BuildingBlocks.Core.Domain;
namespace Explorer.Tours.Core.Domain;

public enum TourExecutionStatus
{
    InProgress,
    Completed,
    Abandoned
}

public class TourExecution : Entity
{
    public long TouristId { get; private set; }
    public long TourId { get; private set; }
    public TourExecutionStatus Status { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public DateTime LastActivity { get; private set; }
    public double PercentageCompleted { get; private set; }
    public int CurrentKeypointSequence { get; private set; }
    public List<KeypointProgress> KeypointProgresses { get; private set; } = new();

    // For EF Core
    protected TourExecution() { }

    public TourExecution(long touristId, long tourId)
    {
        if (touristId <= 0) throw new ArgumentException("Invalid tourist ID");
        if (tourId <= 0) throw new ArgumentException("Invalid tour ID");

        TouristId = touristId;
        TourId = tourId;
        Status = TourExecutionStatus.InProgress;
        StartTime = DateTime.UtcNow;
        LastActivity = DateTime.UtcNow;
        PercentageCompleted = 0;
        CurrentKeypointSequence = 1; // Starts with first keypoint
        Validate();
    }

    private void Validate()
    {
        if (TouristId <= 0) throw new ArgumentException("Invalid tourist ID");
        if (TourId <= 0) throw new ArgumentException("Invalid tour ID");
    }

    public void UpdateLastActivity()
    {
        LastActivity = DateTime.UtcNow;
    }

    public void UpdatePercentageCompleted(double percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentException("Percentage must be between 0 and 100");

        PercentageCompleted = percentage;
        UpdateLastActivity();
    }

    public bool ReachKeypoint(long keypointId, int totalKeypoints)
    {
        if (!IsActive())
            throw new InvalidOperationException("Can only reach keypoints on an active tour");

        if (KeypointProgresses.Any(kp => kp.KeypointId == keypointId))
            return false;

        var progress = new KeypointProgress(keypointId);
        KeypointProgresses.Add(progress);

        CurrentKeypointSequence++;
        UpdatePercentageCompleted((double)KeypointProgresses.Count / totalKeypoints * 100);

        // If all keypoints reached, complete tour
        if (CurrentKeypointSequence > totalKeypoints)
        {
            Complete();
        }
        return true;
    }

    public void Complete()
    {
        if (Status != TourExecutionStatus.InProgress)
            throw new InvalidOperationException("Can only complete a tour that is in progress");

        Status = TourExecutionStatus.Completed;
        EndTime = DateTime.UtcNow;
        PercentageCompleted = 100;
        UpdateLastActivity();
    }

    public void Abandon()
    {
        if (Status != TourExecutionStatus.InProgress)
            throw new InvalidOperationException("Can only abandon a tour that is in progress");

        Status = TourExecutionStatus.Abandoned;
        EndTime = DateTime.UtcNow;
        UpdateLastActivity();
    }

    public bool IsActive()
    {
        return Status == TourExecutionStatus.InProgress;
    }

    public bool CanLeaveReview()
    {
        var oneWeekAgo = DateTime.UtcNow.AddDays(-7);
        return PercentageCompleted > 35 && LastActivity >= oneWeekAgo;
    }
}
