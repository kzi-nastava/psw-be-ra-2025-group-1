namespace Explorer.Payments.Core.Domain.External;

public enum TourPublishStatus { Draft = 0, Published = 1, Archived = 2 }

public record TourInfo(long Id, long CreatorId, string Name, decimal Price, TourPublishStatus Status);

public interface ITourInfoService
{
    TourInfo GetById(long tourId);
}
