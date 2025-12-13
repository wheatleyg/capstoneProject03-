// FOR DEBUG ONLY

using CapstoneBackend.Core.Services;
using CapstoneBackend.Core.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace CapstoneBackend.Core.Controllers;


[Route("facttags")]
public class FactTagsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly FactTagsService _factTagService;


    public FactTagsController(
        IConfiguration configuration,
        FactTagsService factTagService)
    {
        _configuration = configuration;
        _factTagService = factTagService;
    }


    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            var result = _factTagService.GetAll();
            return Ok(result);
        }
        catch  (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
    
}