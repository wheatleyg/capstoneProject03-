
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
            var allFacts = GetFactsFromService(randomService);

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
                if (!genre.Visible) continue;

                genreMap[genreId] = genre;
                allFacts.AddRange(GetFactsFromServiceByGenreId(genre, genreId));
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
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }


    [HttpGet("getByTag/{tag}/{count}")]
    public IActionResult GetByTag(string tag, int count = 5)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return BadRequest(new { Message = "Tag cannot be empty." });
            }

            if (count <= 0 || count > 100)
            {
                return BadRequest(new { Message = "Count must be between 1 and 100" });
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
                if (!genre.Visible) continue;

                genreMap[genreId] = genre;
                allFacts.AddRange(GetFactsFromServiceByGenreId(genre, genreId));
            }

            if (!allFacts.Any())
            {
                return NotFound(new { Message = $"No Facts found for tag '{tag}'" });
            }


            var shuffledFacts = allFacts.OrderBy(x => Random.Shared.Next()).ToList();
            var factsToReturn = shuffledFacts.Take(Math.Min(count, shuffledFacts.Count)).ToList();

            var responses = new List<FactResponse>();
            foreach (var fact in factsToReturn)
            {
                int factGenreId = fact switch
                {
                    CatDb cat => cat.GenreId,
                    SpaceDb space => space.GenreId,
                    _ => 0
                };

                if (genreMap.ContainsKey(factGenreId))
                {
                    responses.Add(BuildFactResponse(fact, genreMap[factGenreId]));
                }
            }

            return Ok(responses);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("getTags")]
    public IActionResult GetAllTags()
    {
        try
        {
            var allFactTags = _factTagsService.GetAll();
            var allTags = allFactTags
                .Where(ft => ft.AvailableTags != null)
                .SelectMany(ft => ft.AvailableTags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t)
                .ToList();

            return Ok(allTags);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("getTagsByGenre/{genreName}")]
    public IActionResult GetTagsByGenre(string genreName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(genreName))
            {
                return BadRequest(new { Message = "Genre name cannot be empty." });
            }

            var genre = _mainService.GetAll()
                .FirstOrDefault(g => g.GenreName.Equals(genreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null)
            {
                return NotFound(new { Message = $"Genre '{genreName}' not found or not visible" });
            }

            var factTags = _factTagsService.GetAll()
                .FirstOrDefault(ft => ft.GenreId == genre.Id);

            if (factTags == null || factTags.AvailableTags == null || !factTags.AvailableTags.Any())
            {
                return NotFound(new { Message = $"No tags found for genre {genre.GenreName}" });
            }

            return Ok(factTags.AvailableTags);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("getGenres")]
    public IActionResult GetGenres()
    {
        try
        {
            var genres = _mainService.GetAll()
                .Where(g => g.Visible)
                .Select(g => new
                {
                    Id = g.Id,
                    GenreName = g.GenreName,
                    Description = g.Description,
                    TableName = g.TableName
                })
                .OrderBy(g => g.GenreName)
                .ToList();

            return Ok(genres);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }
    
    
//HELPER METHODS
    private List<object> GetFactsFromService(Main service)
    {
        var facts = new List<object>();
        
        if (service.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
        {
            facts.AddRange(_catDbService.GetAll().Cast<object>());
        }
        else if (service.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
        {
            facts.AddRange(_spaceDbService.GetAll().Cast<object>());
        }
        
        return facts;
    }

    private List<object> GetFactsFromServiceByGenreId(Main service, int genreId)
    {
        var facts = new List<object>();
        
        if (service.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
        {
            facts.AddRange(_catDbService.GetAll()
                .Where(f => f.GenreId == genreId)
                .Cast<object>());
        }
        else if (service.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
        {
            facts.AddRange(_spaceDbService.GetAll()
                .Where(f => f.GenreId == genreId)
                .Cast<object>());
        }
        
        return facts;
    }

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