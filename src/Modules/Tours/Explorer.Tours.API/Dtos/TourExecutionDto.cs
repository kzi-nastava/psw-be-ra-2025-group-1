namespace Explorer.Tours.API.Dtos;

public enum TourExecutionStatusDto
{
    InProgress,
    Completed,
    Abandoned
}

public class TourExecutionDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public TourExecutionStatusDto Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime LastActivity { get; set; }
    public double PercentageCompleted { get; set; }
    public int CurrentKeypointSequence { get; private set; }
}

public class StartTourDto
{
    public long TourId { get; set; }
    public double InitialLatitude { get; set; }
    public double InitialLongitude { get; set; }
}
