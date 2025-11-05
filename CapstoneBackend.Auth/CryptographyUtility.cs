using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using CapstoneBackend.Utilities.Exceptions;


[assembly:InternalsVisibleTo("CapstoneBackend.Test")]

namespace CapstoneBackend.Auth;

internal static class CryptographyUtility
{
    internal static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt) {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    internal static bool VerifyPasswordHash(string password, byte[]? storedHash, byte[]? storedSalt) {
        if (storedHash is null || storedSalt is null)
        {
            throw new BadRequestException("Password salt or hash missing.")
            {
                ClientMessage = "An error has occurred while logging you in."
            };
        }
        using var hmac = new HMACSHA512(storedSalt);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        //check if any there are any elements where the values don't match
        //    vvv This is because we DON'T want to find any
        return !hash.Where((t, i) => !t.Equals(storedHash[i])).Any();
    }
}