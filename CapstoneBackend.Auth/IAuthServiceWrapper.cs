using CapstoneBackend.Auth.Models;

namespace CapstoneBackend.Auth;

public interface IAuthServiceWrapper
{
    internal bool VerifyPasswordHash(string password, byte[]? storedHash, byte[]? storedSalt);
    internal string CreateToken(DatabaseUser user);
}