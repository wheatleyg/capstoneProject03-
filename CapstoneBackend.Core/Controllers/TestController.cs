using System.Collections;
using CapstoneBackend.Auth;
using CapstoneBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneBackend.Core.Controllers;

public class TestController : Controller
{
    private readonly ILogger<TestController> _logger;
    private readonly IConfiguration _configuration;
    private readonly DbConnectionTest _dbConnectionTest;
    private readonly IUserContext _userContext;

    public TestController(
        ILogger<TestController> logger,
        IConfiguration configuration,
        DbConnectionTest dbConnectionTest,
        IUserContext userContext)
    {
        _logger = logger;
        _configuration = configuration;
        _dbConnectionTest = dbConnectionTest;
        _userContext = userContext;
    }

    [HttpGet("test/online")]
    public IActionResult TestOnline()
    {
        _logger.LogInformation("test endpoint called");
        return Ok("Backend online.");
    }

    [HttpGet("test/environmentVariables")]
    public IActionResult TestEnvironmentVariables()
    {
        _logger.LogInformation("environment variable endpoint called");
        
        if (_configuration.GetValue<bool>(EnvironmentVariables.TRUE_TEST_KEY) &&
            !_configuration.GetValue<bool>(EnvironmentVariables.FALSE_TEST_KEY))
            return Ok("Environment variables working.");
        else
            return StatusCode(501, "Environment variables not working.");
    }

    [HttpGet("test/databaseConnection")]
    public async Task<IActionResult> TestConnectionString()
    {
        _logger.LogInformation("connection string endpoint called");
        try
        {
            if (await _dbConnectionTest.TestConnection())
                return Ok("Database connection working.");
            else
                return StatusCode(501, "Database connection not working.");
        }
        catch
        {
            return StatusCode(501, "Database connection not working.");
        }
    }
    
    [HttpGet("test/authentication")]
    [Authorize]
    public IActionResult TestAuth()
    {
        _logger.LogInformation("authentication test called");
        try
        {
            if (_userContext.IsAuthenticated())
                return Ok($"Auth is working. User id is {_userContext.GetUserId()}");
            else
                return StatusCode(501, "Auth not working.");
        }
        catch
        {
            return StatusCode(501, "Auth not working.");
        }
    }
}