using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace CapstoneBackend.Utilities;


public class DbConnectionTest
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbConnectionTest> _logger;

    public DbConnectionTest(IConfiguration configuration, ILogger<DbConnectionTest> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> TestConnection()
    {
        var connectionString = _configuration.GetValue<string>(EnvironmentVariables.MYSQL_CONNECTION_STRING);
        _logger.LogInformation("Making mysql connection");
        await using var connection = new MySqlConnection(connectionString);

        var test = await connection.QueryFirstAsync<int>("SELECT 1;");

        return test == 1;
    }
}