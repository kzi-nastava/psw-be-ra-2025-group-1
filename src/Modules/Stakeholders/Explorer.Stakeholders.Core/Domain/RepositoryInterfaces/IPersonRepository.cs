namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPersonRepository
{
    Person GetByUserId(long userId);
    Person Create(Person person);
}