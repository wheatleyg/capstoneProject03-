using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Services;
using Microsoft.AspNetCore.Mvc;


namespace CapstoneBackend.Core.Controllers;
//JUST FOR DEBUG- I NEED TO KNOW IF I'M AN IDIOT OR NOT 

[Route("media")]
public class MediaController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly MediaService _mediaService;

    public MediaController(
        IConfiguration configuration,
        MediaService mediaService)
    {
        _configuration = configuration;
        _mediaService = mediaService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var result = _mediaService.GetAll();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred." });
        }
    }
    
}