namespace Explorer.Stakeholders.API.Internal;

public interface IInternalUserService
{
    long? GetUserIdByUsername(string username);
    Dictionary<long, string> GetUsernamesByIds(IEnumerable<long> ids);
}