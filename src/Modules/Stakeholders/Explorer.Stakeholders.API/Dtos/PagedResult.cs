namespace Explorer.Stakeholders.API.Dtos;

public class PagedResult<T>         //paginater rezultat za admin listu
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }   // 1-based
    public int Size { get; set; }
}
