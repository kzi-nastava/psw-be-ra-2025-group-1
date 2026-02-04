namespace Explorer.Stakeholders.API.Dtos.Passkey;

public class PasskeyLoginCompleteDto
{
    public string AssertionResponse { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}
