using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Nullbox.Security.Api.Services;
using Nullbox.Security.Application.Common.Interfaces;

[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace Nullbox.Security.Api.Configuration;

[IntentIgnore]
public static class ApplicationSecurityConfiguration
{
    public static IServiceCollection ConfigureApplicationSecurity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        services.AddHttpContextAccessor();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.IncludeErrorDetails = true;

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                            if (!string.IsNullOrEmpty(token))
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var jwtToken = handler.ReadJwtToken(token);

                                // Create a ClaimsPrincipal from the token without validation
                                var identity = new ClaimsIdentity(jwtToken.Claims, JwtBearerDefaults.AuthenticationScheme);
                                context.Principal = new ClaimsPrincipal(identity);
                                context.Success();
                            }

                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            // Optional: Add additional validation or logging here
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // Suppress exceptions if needed
                            context.NoResult();
                            return Task.CompletedTask;
                        }
                    };
                });

        services.AddAuthorization(ConfigureAuthorization);

        return services;
    }

    private static void ConfigureAuthorization(AuthorizationOptions options)
    {
        // Configure policies and other authorization options here. For example:
        // options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("role", "employee"));
        // options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
    }
}