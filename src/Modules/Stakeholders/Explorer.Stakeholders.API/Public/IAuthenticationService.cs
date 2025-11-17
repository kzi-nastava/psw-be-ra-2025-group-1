using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IAuthenticationService
{
    AuthenticationTokensDto Login(CredentialsDto credentials);
    AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account);
    AuthenticationTokensDto RegisterAdmin(AccountRegistrationDto account);
    AuthenticationTokensDto RegisterAuthor(AccountRegistrationDto account);

}