using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class PasskeyCredential : Entity
{
    public long UserId { get; private set; }
    public byte[] CredentialId { get; private set; }
    public byte[] PublicKey { get; private set; }
    public uint SignatureCounter { get; private set; }
    public string CredentialType { get; private set; }
    public Guid AaGuid { get; private set; }
    public string? DeviceName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public bool IsActive { get; private set; }

    private PasskeyCredential() { }

    public PasskeyCredential(
        long userId,
        byte[] credentialId,
        byte[] publicKey,
        uint signatureCounter,
        string credentialType,
        Guid aaGuid,
        string? deviceName)
    {
        UserId = userId;
        CredentialId = credentialId;
        PublicKey = publicKey;
        SignatureCounter = signatureCounter;
        CredentialType = credentialType;
        AaGuid = aaGuid;
        DeviceName = deviceName;
        CreatedAt = DateTime.UtcNow;
        LastUsedAt = null;
        IsActive = true;
        Validate();
    }

    private void Validate()
    {
        if (UserId <= 0) throw new ArgumentException("Invalid UserId");
        if (CredentialId == null || CredentialId.Length == 0) throw new ArgumentException("Invalid CredentialId");
        if (PublicKey == null || PublicKey.Length == 0) throw new ArgumentException("Invalid PublicKey");
        if (string.IsNullOrWhiteSpace(CredentialType)) throw new ArgumentException("Invalid CredentialType");
    }

    public void UpdateSignatureCounter(uint newCounter)
    {
        SignatureCounter = newCounter;
        LastUsedAt = DateTime.UtcNow;
    }

    public void Rename(string newDeviceName)
    {
        DeviceName = newDeviceName;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
