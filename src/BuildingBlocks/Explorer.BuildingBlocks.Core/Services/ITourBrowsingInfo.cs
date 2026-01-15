namespace Explorer.BuildingBlocks.Core.Services;

/// <summary>
/// Minimal interface for tour browsing across modules.
/// Allows other modules to check if tours exist and are published without direct dependencies.
/// </summary>
public interface ITourBrowsingInfo
{
    ITourInfo? GetPublishedTourById(long tourId);
}
