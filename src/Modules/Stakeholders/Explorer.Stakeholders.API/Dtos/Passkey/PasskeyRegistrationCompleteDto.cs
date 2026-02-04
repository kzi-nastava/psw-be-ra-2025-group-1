namespace Explorer.Stakeholders.API.Dtos.Passkey;

public class PasskeyRegistrationCompleteDto
{
    public string AttestationResponse { get; set; } = string.Empty;
    public string? DeviceName { get; set; }
}
