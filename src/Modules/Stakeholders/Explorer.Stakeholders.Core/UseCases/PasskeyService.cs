using System.Text;
using System.Text.Json;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Dtos.Passkey;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.Extensions.Caching.Distributed;

namespace Explorer.Stakeholders.Core.UseCases;

public class PasskeyService : IPasskeyService
{
    private readonly IFido2 _fido2;
    private readonly IDistributedCache _cache;
    private readonly IPasskeyCredentialRepository _passkeyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;

    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    };

    public PasskeyService(
        IFido2 fido2,
        IDistributedCache cache,
        IPasskeyCredentialRepository passkeyRepository,
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator)
    {
        _fido2 = fido2;
        _cache = cache;
        _passkeyRepository = passkeyRepository;
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
    }

    public PasskeyRegistrationOptionsResponseDto GetRegistrationOptions(long userId, string username, string? deviceName)
    {
        var user = new Fido2User
        {
            Id = Encoding.UTF8.GetBytes(userId.ToString()),
            Name = username,
            DisplayName = username
        };

        var existingCredentials = _passkeyRepository.GetByUserId(userId)
            .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
            .ToList();

        var options = _fido2.RequestNewCredential(new RequestNewCredentialParams
        {
            User = user,
            ExcludeCredentials = existingCredentials,
            AuthenticatorSelection = new AuthenticatorSelection
            {
                UserVerification = UserVerificationRequirement.Preferred
            },
            AttestationPreference = AttestationConveyancePreference.None
        });

        var cacheKey = $"fido2:reg:{userId}";
        var cacheData = new RegistrationCacheData
        {
            Options = options,
            DeviceName = deviceName
        };
        _cache.SetString(cacheKey, JsonSerializer.Serialize(cacheData), CacheOptions);

        return new PasskeyRegistrationOptionsResponseDto
        {
            Options = options.ToJson()
        };
    }

    public async Task<PasskeyCredentialDto> CompleteRegistrationAsync(long userId, PasskeyRegistrationCompleteDto request)
    {
        var cacheKey = $"fido2:reg:{userId}";
        var cachedJson = _cache.GetString(cacheKey);
        if (string.IsNullOrEmpty(cachedJson))
        {
            throw new InvalidOperationException("Registration session expired or not found");
        }

        var cacheData = JsonSerializer.Deserialize<RegistrationCacheData>(cachedJson)
            ?? throw new InvalidOperationException("Invalid registration session data");

        var attestationResponse = JsonSerializer.Deserialize<AuthenticatorAttestationRawResponse>(request.AttestationResponse)
            ?? throw new ArgumentException("Invalid attestation response");

        IsCredentialIdUniqueToUserAsyncDelegate callback = async (args, cancellationToken) =>
        {
            var existing = _passkeyRepository.GetByCredentialId(args.CredentialId);
            return existing == null;
        };

        var result = await _fido2.MakeNewCredentialAsync(new MakeNewCredentialParams
        {
            AttestationResponse = attestationResponse,
            OriginalOptions = cacheData.Options,
            IsCredentialIdUniqueToUserCallback = callback
        });

        var deviceName = request.DeviceName ?? cacheData.DeviceName ?? "Passkey";

        var credential = new PasskeyCredential(
            userId,
            result.Id,
            result.PublicKey,
            result.SignCount,
            result.Type.ToString(),
            result.AaGuid,
            deviceName);

        _passkeyRepository.Create(credential);
        _cache.Remove(cacheKey);

        return new PasskeyCredentialDto
        {
            Id = credential.Id,
            DeviceName = credential.DeviceName,
            CreatedAt = credential.CreatedAt,
            LastUsedAt = credential.LastUsedAt
        };
    }

    public PasskeyLoginOptionsResponseDto GetLoginOptions(string? username)
    {
        var sessionId = Guid.NewGuid().ToString();
        List<PublicKeyCredentialDescriptor> allowedCredentials = new();

        if (!string.IsNullOrEmpty(username))
        {
            var user = _userRepository.GetActiveByName(username);
            if (user != null)
            {
                allowedCredentials = _passkeyRepository.GetByUserId(user.Id)
                    .Select(c => new PublicKeyCredentialDescriptor(c.CredentialId))
                    .ToList();
            }
        }

        var options = _fido2.GetAssertionOptions(new GetAssertionOptionsParams
        {
            AllowedCredentials = allowedCredentials,
            UserVerification = UserVerificationRequirement.Preferred
        });

        var cacheKey = $"fido2:login:{sessionId}";
        _cache.SetString(cacheKey, options.ToJson(), CacheOptions);

        return new PasskeyLoginOptionsResponseDto
        {
            Options = options.ToJson(),
            SessionId = sessionId
        };
    }

    public async Task<AuthenticationTokensDto> CompleteLoginAsync(PasskeyLoginCompleteDto request)
    {
        var cacheKey = $"fido2:login:{request.SessionId}";
        var cachedOptionsJson = _cache.GetString(cacheKey);
        if (string.IsNullOrEmpty(cachedOptionsJson))
        {
            throw new InvalidOperationException("Login session expired or not found");
        }

        var options = AssertionOptions.FromJson(cachedOptionsJson);

        var assertionResponse = JsonSerializer.Deserialize<AuthenticatorAssertionRawResponse>(request.AssertionResponse)
            ?? throw new ArgumentException("Invalid assertion response");

        var storedCredential = _passkeyRepository.GetByCredentialId(assertionResponse.RawId)
            ?? throw new UnauthorizedAccessException("Credential not found");

        IsUserHandleOwnerOfCredentialIdAsync callback = async (args, cancellationToken) => true;

        var result = await _fido2.MakeAssertionAsync(new MakeAssertionParams
        {
            AssertionResponse = assertionResponse,
            OriginalOptions = options,
            StoredPublicKey = storedCredential.PublicKey,
            StoredSignatureCounter = storedCredential.SignatureCounter,
            IsUserHandleOwnerOfCredentialIdCallback = callback
        });

        storedCredential.UpdateSignatureCounter(result.SignCount);
        _passkeyRepository.Update(storedCredential);
        _cache.Remove(cacheKey);

        var user = _userRepository.Get(storedCredential.UserId);
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("User account is not active");
        }

        long personId;
        try
        {
            personId = _userRepository.GetPersonId(user.Id);
        }
        catch (KeyNotFoundException)
        {
            personId = 0;
        }

        return _tokenGenerator.GenerateAccessToken(user, personId);
    }

    public List<PasskeyCredentialDto> GetUserPasskeys(long userId)
    {
        return _passkeyRepository.GetByUserId(userId)
            .Select(c => new PasskeyCredentialDto
            {
                Id = c.Id,
                DeviceName = c.DeviceName,
                CreatedAt = c.CreatedAt,
                LastUsedAt = c.LastUsedAt
            })
            .ToList();
    }

    public PasskeyCredentialDto RenamePasskey(long userId, long passkeyId, string newDeviceName)
    {
        var credential = _passkeyRepository.GetByIdAndUserId(passkeyId, userId)
            ?? throw new KeyNotFoundException("Passkey not found");

        credential.Rename(newDeviceName);
        _passkeyRepository.Update(credential);

        return new PasskeyCredentialDto
        {
            Id = credential.Id,
            DeviceName = credential.DeviceName,
            CreatedAt = credential.CreatedAt,
            LastUsedAt = credential.LastUsedAt
        };
    }

    public void DeletePasskey(long userId, long passkeyId)
    {
        var credential = _passkeyRepository.GetByIdAndUserId(passkeyId, userId)
            ?? throw new KeyNotFoundException("Passkey not found");

        credential.Deactivate();
        _passkeyRepository.Update(credential);
    }

    private class RegistrationCacheData
    {
        public CredentialCreateOptions Options { get; set; } = null!;
        public string? DeviceName { get; set; }
    }
}
