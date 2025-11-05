using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;


namespace CapstoneBackend.Auth;

internal class AuthRepository : IAuthRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthRepository> _logger;
    public AuthRepository(IConfiguration configuration, ILogger<AuthRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }
    
    async Task<ApiUser> IAuthRepository.Register(ApiUser user)
    {
        //move this to service?
        CryptographyUtility.CreatePasswordHash(user.Password, out var hash, out var salt);

        var dbUser = new DatabaseUser
        {
            CreateDatetime = user.CreateDatetime,
            Username = user.Username,
            EmailAddress = user.EmailAddress,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsDeleted = false
        };
        
        var connectionString = _configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);
        
        //make sure to return the api object, not the database object
        user.Id = await connection.InsertAsync(dbUser);
        _logger.LogInformation($"User created with id: {dbUser.Id}");

        return user;
    }

    async Task<DatabaseUser?> IAuthRepository.GetUserByUsername(string username)
    {
        var query = "SELECT * FROM `Users` WHERE `Username` = @username";
        
        var connectionString = _configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);

        return await connection.QuerySingleOrDefaultAsync<DatabaseUser>(query, new {username = username});
    }
    async Task<DatabaseUser?> IAuthRepository.GetUserByEmail(string email)
    {
        var query = "SELECT * FROM `Users` WHERE `EmailAddress` = @email";
        
        var connectionString = _configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        await using var connection = new MySqlConnection(connectionString);

        return await connection.QuerySingleOrDefaultAsync<DatabaseUser>(query, new {email = email});
    }
}