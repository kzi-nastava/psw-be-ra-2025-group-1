namespace Explorer.Blog.Core.Domain;

public enum BlogStatus
{
    Draft, // default
    Published,
    Archived, 
    Closed, // score < -10
    Active, // score > 100 or commenst > 10
    Famous // score > 500 & comments > 30
}