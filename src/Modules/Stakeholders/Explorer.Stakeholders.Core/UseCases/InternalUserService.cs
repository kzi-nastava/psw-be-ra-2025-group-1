using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class InternalUserService : IInternalUserService
{
    private readonly IUserRepository _users;

    public InternalUserService(IUserRepository users)
    {
        _users = users;
    }

    public long? GetUserIdByUsername(string username)
    {
        var u = _users.FindByUsername(username);
        return u?.Id;
    }

    public Dictionary<long, string> GetUsernamesByIds(IEnumerable<long> ids)
    {
        var list = _users.GetByIds(ids);
        return list.ToDictionary(x => x.Id, x => x.Username);
    }
}
