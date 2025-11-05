using CapstoneBackend.Auth.Models;

namespace CapstoneBackend.Auth;

internal class AuthServiceWrapper : IAuthServiceWrapper
{
    bool IAuthServiceWrapper.VerifyPasswordHash(string password, byte[]? storedHash, byte[]? storedSalt)
    {
        return CryptographyUtility.VerifyPasswordHash(password, storedHash, storedSalt);
    }

    public string CreateToken(DatabaseUser user)
    {
        return ClaimUtility.CreateToken(user);
    }
}