using System.Runtime.CompilerServices;
using CapstoneBackend.Auth.Models;

[assembly:InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly:InternalsVisibleTo("CapstoneBackend.Test")]

namespace CapstoneBackend.Auth;
public interface IAuthRepository
{
    
    internal Task<ApiUser> Register(ApiUser databaseUser);
    internal Task<DatabaseUser?> GetUserByUsername(string username);
    internal Task<DatabaseUser?> GetUserByEmail(string email);
}