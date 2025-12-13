
/*
 * This is the main 'main' controller.
 * It handles literally everything the frontend will talk to.
 * It will connect all the services into a single, manageable endpoint.
 * The main purpose of the other controllers is to simplify updating-
 * -since it'll already be a lot of link all these services together for Get requests.
 * 
 */

using CapstoneBackend.Core.Models;
using CapstoneBackend.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneBackend.Core.Controllers;



[Route("api")]
public class MainController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly MainService _mainService;
    private readonly FactTagsService _factTagsService;
    private readonly CatDbService _catDbService;
    private readonly SpaceDbService _spaceDbService;
    private readonly MediaService _mediaService;


    public MainController(
        IConfiguration configuration,
        MainService mainService,
        FactTagsService factTagsService,
        CatDbService catDbService,
        SpaceDbService spaceDbService,
        MediaService mediaService)
    {
        _configuration = configuration;
        _mainService = mainService;
        _factTagsService = factTagsService;
        _catDbService = catDbService;
        _spaceDbService = spaceDbService;
        _mediaService = mediaService;
    }


    [HttpGet("getRandomFact")]
    public IActionResult GetRandomFact()
    {
        try
        {
            var services = _mainService.GetAll().Where(s => s.Visible).ToList();
            if (!services.Any())
            {
                return NotFound(new { Message = "No Facts found" });
            }

            var randomService = services[Random.Shared.Next(services.Count)];

            List<object> allFacts = new();

            if (randomService.TableName.Equals("catdb", StringComparison.OrdinalIgnoreCase))
            {
                allFacts.AddRange(_catDbService.GetAll().Cast<object>());
            }
            else if (randomService.TableName.Equals("spacedb", StringComparison.OrdinalIgnoreCase))
            {
                allFacts.AddRange(_spaceDbService.GetAll().Cast<object>());
            }


            if (!allFacts.Any())
            {
                return NotFound(new { Message = $"No Facts found in {randomService.TableName}" });
            }

            var randomFact = allFacts[Random.Shared.Next(allFacts.Count)];

            var response = BuildFactResponse(randomFact, randomService);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getByTag/{tag}")]
    public IActionResult GetByTag(string tag)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest(new { Message = "Tag cannot be empty." });
            }

            tag = tag.Trim().ToLowerInvariant();
            var allFactTags = _factTagsService.GetAll();
            var matchingGenres = allFactTags
                .Where(ft => ft.AvailableTags != null &&
                             ft.AvailableTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                .Select(ft => ft.GenreId)
                .Distinct()
                .ToList();

            if (!matchingGenres.Any())
            {
                return NotFound($"No Genre found for '{tag}'");
            }

            var allFacts = new List<object>();
            var genreMap = new Dictionary<int, Main>();

            foreach (var genreId in matchingGenres)
            {
                var genre = _mainService.GetEntryById(genreId);
                if (!genre.Visible)
                {
                    continue;
                }

                genreMap[genreId] = genre;

                if (genre.TableName.Equals("catdb", StringComparison.OrdinalIgnoreCase))
                {
                    allFacts.AddRange(_catDbService.GetAll()
                        .Where(f => f.GenreId == genreId)
                        .Cast<object>());
                }
                else if (genre.TableName.Equals("spacedb", StringComparison.OrdinalIgnoreCase))
                {
                    allFacts.AddRange(_spaceDbService.GetAll()
                        .Where(f => f.GenreId == genreId)
                        .Cast<object>());
                }
            }

            if (!allFacts.Any())
            {
                return NotFound(new { Message = $"No Facts found for tag '{tag}'" });
            }

            var randomFact = allFacts[Random.Shared.Next(allFacts.Count)];

            int factGenreId = randomFact switch
            {
                CatDb cat => cat.GenreId,
                SpaceDb space => space.GenreId,
                _ => 0
            };

            var factGenre = genreMap[factGenreId];
            var response = BuildFactResponse(randomFact, factGenre);

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erorr:  {ex.Message}");
        }
    }
    
    
//HELPER
    private FactResponse BuildFactResponse(object fact, Main service)
    {
        int factId = 0;
        int genreId = 0;
        string factText = string.Empty;
        int sourceId = 0;

        if (fact is CatDb catFact)
        {
            factId = catFact.Id;
            genreId = catFact.GenreId;
            factText = catFact.FactText;
            sourceId = catFact.SourceId; //ever get that feeling when u code so long words start looking like u spelt them wrong?
        }
        else if (fact is SpaceDb spaceFact)
        {
            factId = spaceFact.Id;
            genreId = spaceFact.GenreId;
            factText = spaceFact.FactText;
            sourceId = spaceFact.SourceId;
        }

        Media? media = null;
        try
        {
            if (sourceId > 0)
            {
                media = _mediaService.GetEntryById(sourceId);
            }
        }
        catch
        {
            //no found
        }

        return new FactResponse
        {
            Id = factId,
            GenreId = genreId,
            GenreName = service.GenreName,
            FactText = factText,
            Media = media,
            SourceTable = service.TableName
        };
    }
    
}