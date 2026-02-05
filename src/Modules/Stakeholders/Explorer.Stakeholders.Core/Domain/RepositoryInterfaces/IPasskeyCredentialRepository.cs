namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPasskeyCredentialRepository
{
    PasskeyCredential? GetByCredentialId(byte[] credentialId);
    List<PasskeyCredential> GetByUserId(long userId);
    PasskeyCredential? GetByIdAndUserId(long id, long userId);
    PasskeyCredential Create(PasskeyCredential credential);
    PasskeyCredential Update(PasskeyCredential credential);
}
