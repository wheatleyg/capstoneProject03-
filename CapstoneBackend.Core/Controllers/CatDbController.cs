using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;



namespace CapstoneBackend.Core.Controllers;

[Route("catdb")]
public class CatDbController: Controller
{
 private readonly IConfiguration _configuration;
 private readonly CatDbService _catDbService;
 private readonly ILogger<CatDbController> _logger;


 public CatDbController(
     IConfiguration configuration,
     CatDbService catDbService,
     ILogger<CatDbController> logger)
 {
     _configuration = configuration;
     _catDbService = catDbService;
     _logger = logger;
 }

 [HttpPost]
 public IActionResult CreateEntry([FromBody] CatDb catDb)
 {
     try
     {
         var created = _catDbService.CreateEntry(catDb);
         return CreatedAtAction(nameof(CatDbById), new { id = created.Id }, created); //no clue what this does.
     }
     catch (ArgumentException ex)
     {
         _logger.LogError(ex, ex.Message);
         return BadRequest(new { message = ex.Message });
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, ex.Message);
         return StatusCode(500, new { message = "An unexpected error occurred." });
     }
 }

 [HttpPut("{id:int}")]
 public IActionResult UpdateEntry([FromRoute] int id, [FromBody] CatDb catDb)
 {
     if (id != catDb.Id)
     {
         return BadRequest(new { message = "Id in route does not match Id in body." });
     }

     try
     {
         var updated = _catDbService.UpdateEntry(catDb);
         return Ok(updated);
     }
     catch (ArgumentOutOfRangeException ex)
     {
         _logger.LogError(ex, ex.Message);
         return BadRequest(new { message = ex.Message });
     }
     catch (KeyNotFoundException ex)
     {
         _logger.LogError(ex, ex.Message);
         return NotFound(new { message = ex.Message });
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, ex.Message);
         return StatusCode(500, new { message = "An unexpected error occurred." });
         
     }
 } 
 
 
 [HttpGet("{id:int}")] //tuff
 public IActionResult CatDbById([FromRoute] int id)
 {
     Console.WriteLine("Trying. . . ");


     try
     {
         var result = _catDbService.GetEntryById(id);
         return Ok(result);
     }
     catch (ArgumentOutOfRangeException ex)
     {
         _logger.LogError(ex, ex.Message);
         return BadRequest(new { message = ex.Message });
     }
     catch (KeyNotFoundException ex)
     {
         _logger.LogError(ex, ex.Message);
         return NotFound(new { message = ex.Message });
     }
     catch (Exception ex) // catch anything unexpected
     {
         _logger.LogError(ex, ex.Message);
         return StatusCode(500, new { message = "An unexpected error occurred." });
     }
 }

 [HttpGet]
 public IActionResult GetAll()
 {
     try
     {
         var result = _catDbService.GetAll();
         return Ok(result);
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, ex.Message);
         return StatusCode(500, new { message = "An unexpected error occurred." });
     }
 }

 [HttpDelete("{id:int}")]
 public IActionResult DeleteEntryById(int id)
 {
     try
     {
         var result = _catDbService.DeleteEntryById(id);
         return Ok(new { message = $"Deleted ok! (returned: {result})"});
     }
     catch (ArgumentOutOfRangeException ex)
     {
         _logger.LogError(ex, ex.Message);
         return BadRequest(new { message = ex.Message });
     }
     catch (KeyNotFoundException ex)
     {
         _logger.LogError(ex, ex.Message);
         return NotFound(new { message = ex.Message });
     }
     catch (Exception ex)
     {
         _logger.LogError(ex, ex.Message);
         return StatusCode(500, new { message = "An unexpected error occurred." });
     }
 }
}
//Frontend is planned to never actually talk directly to these controllers-
// at least public, it'll still be used for admin use and such to make direct-
//editing easier. Also, I got to meet rubric too :P