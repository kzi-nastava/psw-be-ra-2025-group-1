namespace Explorer.BuildingBlocks.Core.Services;

/// <summary>
/// Minimal interface for cross-module tour information access.
/// Provides basic tour info without exposing the full Tours module API.
/// </summary>
public interface ITourInfo
{
    long Id { get; }
    string Title { get; }
    double Price { get; }
    bool IsPublished { get; }
}
