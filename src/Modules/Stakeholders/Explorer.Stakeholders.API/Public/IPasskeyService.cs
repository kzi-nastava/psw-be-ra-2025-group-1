using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Dtos.Passkey;

namespace Explorer.Stakeholders.API.Public;

public interface IPasskeyService
{
    PasskeyRegistrationOptionsResponseDto GetRegistrationOptions(long userId, string username, string? deviceName);
    Task<PasskeyCredentialDto> CompleteRegistrationAsync(long userId, PasskeyRegistrationCompleteDto request);
    PasskeyLoginOptionsResponseDto GetLoginOptions(string? username);
    Task<AuthenticationTokensDto> CompleteLoginAsync(PasskeyLoginCompleteDto request);
    List<PasskeyCredentialDto> GetUserPasskeys(long userId);
    PasskeyCredentialDto RenamePasskey(long userId, long passkeyId, string newDeviceName);
    void DeletePasskey(long userId, long passkeyId);
}
