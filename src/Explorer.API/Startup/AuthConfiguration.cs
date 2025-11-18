using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Explorer.API.Startup;

public static class AuthConfiguration
{
    public static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        ConfigureAuthentication(services);
        ConfigureAuthorizationPolicies(services);
        return services;
    }

    private static void ConfigureAuthentication(IServiceCollection services)
    {
        var key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "L1uKpZQzI1Yx0+OaS0kXkE7u0n/5Q0U3R5s3FVmXcXU=";
        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "explorer";
        var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "explorer-front.com";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.FromMinutes(5) // Allow some clock drift
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();

                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("AuthenticationTokens-Expired", "true");
                            logger.LogWarning("JWT Token expired: {Message}", context.Exception.Message);
                        }
                        else
                        {
                            logger.LogError("JWT Authentication failed: {ExceptionType} - {Message}",
                                context.Exception.GetType().Name, context.Exception.Message);
                        }

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogInformation("JWT Token successfully validated for user: {Username}",
                            context.Principal.FindFirst("username")?.Value ?? "Unknown");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogWarning("JWT Authentication challenge: {Error} - {ErrorDescription}",
                            context.Error, context.ErrorDescription);
                        return Task.CompletedTask;
                    }
                };
            });
    }

    private static void ConfigureAuthorizationPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("administratorPolicy", policy => policy.RequireRole("administrator"));
            options.AddPolicy("authorPolicy", policy => policy.RequireRole("author"));
            options.AddPolicy("touristPolicy", policy => policy.RequireRole("tourist"));
            options.AddPolicy("authorOrTouristPolicy", policy => policy.RequireRole("author", "tourist"));
        });
    }
}