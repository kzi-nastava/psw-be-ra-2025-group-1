using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class PasskeyCredentialRepository : IPasskeyCredentialRepository
{
    private readonly StakeholdersContext _dbContext;

    public PasskeyCredentialRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public PasskeyCredential? GetByCredentialId(byte[] credentialId)
    {
        return _dbContext.PasskeyCredentials
            .FirstOrDefault(p => p.CredentialId.SequenceEqual(credentialId) && p.IsActive);
    }

    public List<PasskeyCredential> GetByUserId(long userId)
    {
        return _dbContext.PasskeyCredentials
            .Where(p => p.UserId == userId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();
    }

    public PasskeyCredential? GetByIdAndUserId(long id, long userId)
    {
        return _dbContext.PasskeyCredentials
            .FirstOrDefault(p => p.Id == id && p.UserId == userId && p.IsActive);
    }

    public PasskeyCredential Create(PasskeyCredential credential)
    {
        _dbContext.PasskeyCredentials.Add(credential);
        _dbContext.SaveChanges();
        return credential;
    }

    public PasskeyCredential Update(PasskeyCredential credential)
    {
        _dbContext.PasskeyCredentials.Update(credential);
        _dbContext.SaveChanges();
        return credential;
    }
}
