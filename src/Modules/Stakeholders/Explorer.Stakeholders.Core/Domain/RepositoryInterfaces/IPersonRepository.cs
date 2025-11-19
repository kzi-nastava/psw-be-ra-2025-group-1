namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person GetByUserId(long userId);
    Person Create(Person person);
    Person Update(Person person);
    Person Get(long id);
}