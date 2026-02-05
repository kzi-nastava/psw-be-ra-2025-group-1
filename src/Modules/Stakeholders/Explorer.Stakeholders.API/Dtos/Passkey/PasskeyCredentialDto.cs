namespace Explorer.Stakeholders.API.Dtos.Passkey;

public class PasskeyCredentialDto
{
    public long Id { get; set; }
    public string? DeviceName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
}
