namespace Explorer.Payments.Core.Domain.External;
public enum TourPublishStatus
{
    Draft,
    Published,
    Archived
}


public record TourInfo(long Id, string Name, double Price, TourPublishStatus Status);

public interface ITourInfoService
{
    TourInfo GetById(long tourId);
}