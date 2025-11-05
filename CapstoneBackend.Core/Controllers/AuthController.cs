using System.Net.Mime;
using CapstoneBackend.Auth;
using CapstoneBackend.Auth.Models;
using CapstoneBackend.Utilities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneBackend.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IAuthService  _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Register([FromBody] ApiUser user)
    {
        try
        {
            var newUser = await _authService.Register(user);
            newUser.Password = ""; //don't send the password back in plain text
            return Ok(newUser);
        }
        catch (BaseCapstoneException ex)
        {
            _logger.LogError(ex, ex.Message);
            return ex.ReturnDefaultResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(new {message = "Registration failed" });
        }
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Login([FromBody] Login user)
    {
        try
        {
            var token = await _authService.Login(user);
            return Ok(token);
        }
        catch (BaseCapstoneException ex)
        {
            _logger.LogError(ex, ex.Message);
            return ex.ReturnDefaultResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(new {message = "Login failed"});
        }
    }
}