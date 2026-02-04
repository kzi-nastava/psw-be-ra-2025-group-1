namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;


public interface IJournalRepository
{
    Journal Add(Journal journal);
    Journal? GetById(long id);
    IEnumerable<Journal> GetByUserId(long userId);

    void Update(Journal journal);
    void Delete(Journal journal);

    (IReadOnlyList<Journal> Items, int TotalCount) GetPaged(int page, int size); // Za admin listu

    Journal? GetSingleByUserId(long userId); // Ako ikada treba "jedan" dnevnik

    IEnumerable<Journal> GetAccessibleByUserId(long userId);

    Journal? GetByPublishedBlogId(long blogId);


}

