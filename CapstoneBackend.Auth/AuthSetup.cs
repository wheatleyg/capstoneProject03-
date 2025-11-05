using System.Net;
using System.Text;
using CapstoneBackend.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneBackend.Auth;

public static class AuthSetup
{
    public static IServiceCollection AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddHttpContextAccessor();
        
        services.AddSingleton<ClaimUtility>(new ClaimUtility(configuration));
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthServiceWrapper, AuthServiceWrapper>();
        services.AddScoped<IAuthService, AuthService>();
        
        
        services.AddAuthentication(configuration);
        services.AddAuthorization();
        
        return services;
    }
    
    private static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.ASCII.GetBytes(configuration[EnvironmentVariables.TOKEN_KEY]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Events = SetupBearerEvents();
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                
            };
        });
    }

    private static JwtBearerEvents SetupBearerEvents()
    {
        return new JwtBearerEvents
        {
            //when everything goes well
            OnTokenValidated = context =>
            {
                return Task.CompletedTask;
            },
            //get called anytime there's an auth error
            //this is mainly used when an auth token is missing (for now)
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                if (context.AuthenticateFailure?.GetType() == typeof(SecurityTokenMalformedException) ||
                    context.AuthenticateFailure?.GetType() == typeof(SecurityTokenExpiredException))
                    return;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Auth token is missing.", //not sure if this is the only time we'll see this issue
                    redirectLink = "api/auth/login"
                });
            },
            //gets called when there's an issue with the token
            OnAuthenticationFailed = context =>
            {
                var errorMessage = "";
                if (context.Exception.GetType() == typeof(SecurityTokenMalformedException))
                    errorMessage = "Auth token is malformed.";
                else if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    errorMessage = "Auth token is expired.";
                context.Response.OnStarting(async () =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = errorMessage,
                        redirectLink = "api/auth/login"
                    });
                });
                
                return Task.CompletedTask;
            }
            //TODO add more bearer events for better error messaging
        };
    }
}