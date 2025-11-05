using CapstoneBackend.Auth.Models;

namespace CapstoneBackend.Auth;

public interface IAuthService
{
    public Task<ApiUser> Register(ApiUser user);
    public Task<AuthToken> Login(Login login);
}