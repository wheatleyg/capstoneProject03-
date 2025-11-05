using System.Security.Claims;
using Dapper.Extensions;
using Microsoft.AspNetCore.Http;

namespace CapstoneBackend.Auth;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _contextAccessor;

    public UserContext(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public bool IsAuthenticated()
    {
        return GetUserId() != -1;
    }

    public int GetUserId()
    {
        var userClaims = _contextAccessor.HttpContext!.User.Claims.ToList();

        var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

        if (userId is null || userId.IsNullOrWhiteSpace()) return -1;
        
        return int.Parse(userId);
    }
    
    
}