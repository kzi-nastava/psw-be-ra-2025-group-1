using System.Net.Http.Json;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.Tests.Helpers;

public static class AuthHelpers
{
    public static async Task<string> LoginAndGetTokenAsync(HttpClient client, string username, string password)
    {
        var res = await client.PostAsJsonAsync("/api/users/login", new CredentialsDto
        {
            Username = username,
            Password = password
        });
        res.EnsureSuccessStatusCode();
        var tokens = await res.Content.ReadFromJsonAsync<AuthenticationTokensDto>();
        return tokens!.AccessToken;
    }
}
