namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User Create(User user);
    long GetPersonId(long userId);
    List<User> GetAll();
    User Get(long id);
    User Update(User user);

    User? FindByUsername(string username); //za kolaboratore
}