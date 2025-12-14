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
    private readonly ILogger<MainController> _logger;


    public MainController(
        IConfiguration configuration,
        MainService mainService,
        FactTagsService factTagsService,
        CatDbService catDbService,
        SpaceDbService spaceDbService,
        MediaService mediaService,
        ILogger<MainController> logger)
    {
        _configuration = configuration;
        _mainService = mainService;
        _factTagsService = factTagsService;
        _catDbService = catDbService;
        _spaceDbService = spaceDbService;
        _mediaService = mediaService;
        _logger = logger;
    }


    [HttpGet("getRandomFact")]
    public IActionResult GetRandomFact()
    {
        try
        {
            var services = _mainService.GetAll().Where(s => s.Visible).ToList();
            if (!services.Any()) return NotFound(new { Message = "No Facts found" });

            var randomService = services[Random.Shared.Next(services.Count)];
            var allFacts = GetFactsWithDetailsFromService(randomService);

            if (!allFacts.Any()) return NotFound(new { Message = $"No Facts found in {randomService.TableName}" });

            var randomFact = allFacts[Random.Shared.Next(allFacts.Count)];
            return Ok(randomFact);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("getByTag/{tag}")]
    public IActionResult GetByTag(string tag, [FromQuery] int? count = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tag)) return BadRequest(new { Message = "Tag cannot be empty." });

            // If count is provided, validate it
            if (count.HasValue && (count.Value <= 0 || count.Value > 100))
                return BadRequest(new { Message = "Count must be between 1 and 100" });

            tag = tag.Trim().ToLowerInvariant();
            var allFactTags = _factTagsService.GetAll();
            var matchingGenres = allFactTags
                .Where(ft => ft.AvailableTags != null &&
                             ft.AvailableTags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
                .Select(ft => ft.GenreId)
                .Distinct()
                .ToList();

            if (!matchingGenres.Any()) return NotFound($"No Genre found for '{tag}'");

            var allFacts = new List<FactResponse>();
            var genreMap = new Dictionary<int, Main>();

            foreach (var genreId in matchingGenres)
            {
                var genre = _mainService.GetEntryById(genreId);
                if (!genre.Visible) continue;

                genreMap[genreId] = genre;
                allFacts.AddRange(GetFactsWithDetailsByGenreId(genre, genreId));
            }

            if (!allFacts.Any()) return NotFound(new { Message = $"No Facts found for tag '{tag}'" });

            // If count is null or 1, return a single random fact
            if (!count.HasValue || count.Value == 1)
            {
                var randomFact = allFacts[Random.Shared.Next(allFacts.Count)];
                return Ok(randomFact);
            }

            // If count > 1, return multiple facts
            var shuffledFacts = allFacts.OrderBy(x => Random.Shared.Next()).ToList();
            var factsToReturn = shuffledFacts.Take(Math.Min(count.Value, shuffledFacts.Count)).ToList();

            return Ok(factsToReturn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { Message = $"Error: {ex.Message}" });
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
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpGet("getTagsByGenre/{genreName}")]
    public IActionResult GetTagsByGenre(string genreName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(genreName))
                return BadRequest(new { Message = "Genre name cannot be empty." });

            var genre = _mainService.GetAll()
                .FirstOrDefault(g => g.GenreName.Equals(genreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null) return NotFound(new { Message = $"Genre '{genreName}' not found or not visible" });

            // Use stored procedure to get fact tags with genre information in one query
            var factTagsWithGenre = _factTagsService.GetFactTagsWithGenre(genre.Id);

            if (factTagsWithGenre == null || !factTagsWithGenre.AvailableTags.Any())
                return NotFound(new { Message = $"No tags found for genre {genre.GenreName}" });

            return Ok(factTagsWithGenre.AvailableTags);
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
                    g.Id,
                    g.GenreName,
                    g.Description,
                    g.TableName
                })
                .OrderBy(g => g.GenreName)
                .ToList();

            return Ok(genres);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost("easyAddFact")]
    public IActionResult EasyAddFact([FromBody] SimpleFactEntry entry)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(entry.FactText))
                return BadRequest(new { Message = "Fact text cannot be empty." });
            if (string.IsNullOrWhiteSpace(entry.MediaLink))
                return BadRequest(new { Message = "Media link cannot be empty." });
            if (string.IsNullOrWhiteSpace(entry.GenreName))
                return BadRequest(new { Message = "Genre name cannot be empty." });
            if (string.IsNullOrWhiteSpace(entry.MediaType))
                return BadRequest(new { Message = "Media type cannot be empty." });

            // Parse MediaType string to enum
            if (!Enum.TryParse<MediaType>(entry.MediaType, true, out var mediaTypeEnum))
                return BadRequest(new { Message = $"Invalid media type '{entry.MediaType}'. Valid values are: Image, Video, Audio, Misc, None" });

            var genre = _mainService.GetAll()
                .FirstOrDefault(g =>
                    g.GenreName.Equals(entry.GenreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null)
                return NotFound(new { Message = $"Genre '{entry.GenreName}' not found or not visible" });


            var existingMedia = _mediaService.GetAll()
                .FirstOrDefault(m => m.Link.Equals(entry.MediaLink, StringComparison.OrdinalIgnoreCase));

            int mediaId;
            if (existingMedia != null)
            {
                mediaId = existingMedia.Id;
            }
            else
            {
                var newMedia = new Media
                {
                    Id = 0,
                    Link = entry.MediaLink,
                    MediaType = mediaTypeEnum
                };
                var createdMedia = _mediaService.CreateEntry(newMedia);
                mediaId = createdMedia.Id;
            }


            if (genre.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
            {
                var catFact = new CatDb
                {
                    Id = 0,
                    GenreId = genre.Id,
                    FactText = entry.FactText,
                    SourceId = mediaId
                };
                var created = _catDbService.CreateEntry(catFact);
                return Ok(new { Message = "Fact added successfully", FactId = created.Id });
            }

            if (genre.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
            {
                var spaceFact = new SpaceDb
                {
                    Id = 0,
                    GenreId = genre.Id,
                    FactText = entry.FactText,
                    SourceId = mediaId
                };
                var created = _spaceDbService.CreateEntry(spaceFact);
                return Ok(new { Message = "Fact added successfully", FactId = created.Id });
            }

            return BadRequest(new { Message = $"Unknown table type: {genre.TableName}" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return NotFound(new { ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { Message = $"Error: {ex.Message}" });
        }
    }

    [HttpPut("easyUpdateFact")]
    public IActionResult EasyUpdateFact([FromBody] SimpleFactUpdate update)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(update.GenreName))
                return BadRequest(new { Message = "Genre name cannot be empty." });


            var genre = _mainService.GetAll()
                .FirstOrDefault(g =>
                    g.GenreName.Equals(update.GenreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null)
                return NotFound(new { Message = $"Genre '{update.GenreName}' not found or not visible" });


            object? existingFact = null;
            if (genre.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
                existingFact = _catDbService.GetEntryById(update.FactId);
            else if (genre.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
                existingFact = _spaceDbService.GetEntryById(update.FactId);
            else
                return BadRequest(new { Message = $"Unknown table type: {genre.TableName}" });

            if (existingFact == null)
                return NotFound(new { Message = $"Fact with ID {update.FactId} not found" });


            var sourceId = existingFact switch
            {
                CatDb cat => cat.SourceId,
                SpaceDb space => space.SourceId,
                _ => 0
            };

            if (!string.IsNullOrWhiteSpace(update.MediaLink) && update.MediaType.HasValue)
            {
                var existingMedia = _mediaService.GetAll()
                    .FirstOrDefault(m => m.Link.Equals(update.MediaLink, StringComparison.OrdinalIgnoreCase));

                if (existingMedia != null)
                {
                    sourceId = existingMedia.Id;
                }
                else
                {
                    var newMedia = new Media
                    {
                        Id = 0,
                        Link = update.MediaLink,
                        MediaType = update.MediaType.Value
                    };
                    var createdMedia = _mediaService.CreateEntry(newMedia);
                    sourceId = createdMedia.Id;
                }
            }


            if (genre.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
            {
                var catFact = existingFact as CatDb;
                var updated = new CatDb
                {
                    Id = catFact!.Id,
                    GenreId = catFact.GenreId,
                    FactText = !string.IsNullOrWhiteSpace(update.FactText) ? update.FactText : catFact.FactText,
                    SourceId = sourceId
                };
                _catDbService.UpdateEntry(updated);
                return Ok(new { Message = "Fact updated successfully" });
            }

            if (genre.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
            {
                var spaceFact = existingFact as SpaceDb;
                var updated = new SpaceDb
                {
                    Id = spaceFact!.Id,
                    GenreId = spaceFact.GenreId,
                    FactText = !string.IsNullOrWhiteSpace(update.FactText) ? update.FactText : spaceFact.FactText,
                    SourceId = sourceId
                };
                _spaceDbService.UpdateEntry(updated);
                return Ok(new { Message = "Fact updated successfully" });
            }

            return BadRequest(new { Message = "Update failed" });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return NotFound(new { ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { Message = $"Error: {ex.Message}" });
        }
    }

    [HttpDelete("easyDeleteFact")]
    public IActionResult EasyDeleteFact([FromBody] SimpleFactDelete delete)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(delete.GenreName))
                return BadRequest(new { Message = "Genre name cannot be empty." });

            var genre = _mainService.GetAll()
                .FirstOrDefault(g =>
                    g.GenreName.Equals(delete.GenreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null)
                return NotFound(new { Message = $"Genre '{delete.GenreName}' not found or not visible" });


            object? factToDelete = null;
            int sourceId = 0;

            if (genre.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
            {
                factToDelete = _catDbService.GetEntryById(delete.FactId);
                if (factToDelete is CatDb catFact)
                    sourceId = catFact.SourceId;
            }
            else if (genre.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
            {
                factToDelete = _spaceDbService.GetEntryById(delete.FactId);
                if (factToDelete is SpaceDb spaceFact)
                    sourceId = spaceFact.SourceId;
            }
            else
            {
                return BadRequest(new { Message = $"Unknown table type: {genre.TableName}" });
            }

            if (factToDelete == null)
                return NotFound(new { Message = $"Fact with ID {delete.FactId} not found" });


            var deleted = false;
            if (genre.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
                deleted = _catDbService.DeleteEntryById(delete.FactId);
            else if (genre.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
                deleted = _spaceDbService.DeleteEntryById(delete.FactId);

            if (!deleted)
                return NotFound(new { Message = $"Fact with ID {delete.FactId} not found" });


            if (delete.DeleteMedia && sourceId > 0)
            {
                var allCatFacts = _catDbService.GetAll();
                var allSpaceFacts = _spaceDbService.GetAll();
                
                var mediaStillInUse = allCatFacts.Any(f => f.SourceId == sourceId) ||
                                     allSpaceFacts.Any(f => f.SourceId == sourceId);

                if (!mediaStillInUse)
                {
                    try
                    {
                        _mediaService.DeleteEntryById(sourceId);
                    }
                    catch
                    {
                        
                    }
                }
            }

            return Ok(new { Message = "Fact deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost("easyAddTags")]
    public IActionResult EasyAddTags([FromBody] SimpleTagAdd tagAdd)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tagAdd.GenreName))
                return BadRequest(new { Message = "Genre name cannot be empty." });
            if (tagAdd.TagsToAdd == null || !tagAdd.TagsToAdd.Any())
                return BadRequest(new { Message = "Tags to add cannot be empty." });


            var genre = _mainService.GetAll()
                .FirstOrDefault(g =>
                    g.GenreName.Equals(tagAdd.GenreName, StringComparison.OrdinalIgnoreCase) && g.Visible);

            if (genre == null)
                return NotFound(new { Message = $"Genre '{tagAdd.GenreName}' not found or not visible" });


            var factTags = _factTagsService.GetAll()
                .FirstOrDefault(ft => ft.GenreId == genre.Id);

            if (factTags == null)
                return NotFound(new { Message = $"No fact tags found for genre '{tagAdd.GenreName}'" });


            var existingTags = factTags.AvailableTags ?? new List<string>();
            var newTags = tagAdd.TagsToAdd
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToLowerInvariant())
                .Where(t => !existingTags.Any(et => et.Equals(t, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (!newTags.Any())
                return Ok(new { Message = "All tags already exist", Tags = existingTags });

            var updatedTags = existingTags.Concat(newTags).ToList();


            var updated = new FactTags
            {
                Id = factTags.Id,
                GenreId = factTags.GenreId,
                AvailableTags = updatedTags
            };

            _factTagsService.UpdateEntry(updated);
            return Ok(new { Message = "Tags added successfully", Tags = updatedTags });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, ex.Message);
            return NotFound(new { ex.Message });
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(new { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode(500, new { Message = $"Error: {ex.Message}" });
        }
    }


//HELPER METHODS
    private List<object> GetFactsFromService(Main service)
    {
        var facts = new List<object>();

        if (service.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
            facts.AddRange(_catDbService.GetAll());
        else if (service.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
            facts.AddRange(_spaceDbService.GetAll());

        return facts;
    }

    private List<FactResponse> GetFactsWithDetailsFromService(Main service)
    {
        var facts = new List<FactResponse>();

        if (service.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
        {
            var catFacts = _catDbService.GetCatFactsWithDetails(service.Id);
            facts.AddRange(catFacts.Select(f => new FactResponse
            {
                Id = f.Id,
                GenreId = f.GenreId,
                GenreName = f.GenreName,
                FactText = f.FactText,
                Media = f.Media,
                SourceTable = f.TableName
            }));
        }
        else if (service.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
        {
            var spaceFacts = _spaceDbService.GetSpaceFactsWithDetails(service.Id);
            facts.AddRange(spaceFacts.Select(f => new FactResponse
            {
                Id = f.Id,
                GenreId = f.GenreId,
                GenreName = f.GenreName,
                FactText = f.FactText,
                Media = f.Media,
                SourceTable = f.TableName
            }));
        }

        return facts;
    }

    private List<FactResponse> GetFactsWithDetailsByGenreId(Main service, int genreId)
    {
        var facts = new List<FactResponse>();

        if (service.TableName.Equals("CatDb", StringComparison.OrdinalIgnoreCase))
        {
            var catFacts = _catDbService.GetCatFactsWithDetails(genreId);
            facts.AddRange(catFacts.Select(f => new FactResponse
            {
                Id = f.Id,
                GenreId = f.GenreId,
                GenreName = f.GenreName,
                FactText = f.FactText,
                Media = f.Media,
                SourceTable = f.TableName
            }));
        }
        else if (service.TableName.Equals("SpaceDb", StringComparison.OrdinalIgnoreCase))
        {
            var spaceFacts = _spaceDbService.GetSpaceFactsWithDetails(genreId);
            facts.AddRange(spaceFacts.Select(f => new FactResponse
            {
                Id = f.Id,
                GenreId = f.GenreId,
                GenreName = f.GenreName,
                FactText = f.FactText,
                Media = f.Media,
                SourceTable = f.TableName
            }));
        }

        return facts;
    }

    private List<object> GetFactsFromServiceByGenreId(Main service, int genreId)
    {
        // Keep this for backward compatibility, but convert to FactResponse objects
        var factsWithDetails = GetFactsWithDetailsByGenreId(service, genreId);
        return factsWithDetails.Cast<object>().ToList();
    }

    private FactResponse BuildFactResponse(object fact, Main service)
    {
        // If it's already a FactResponse (from stored procedure), return it directly
        if (fact is FactResponse factResponse)
            return factResponse;

        // Fallback to old logic for backward compatibility
        var factId = 0;
        var genreId = 0;
        var factText = string.Empty;
        var sourceId = 0;

        if (fact is CatDb catFact)
        {
            factId = catFact.Id;
            genreId = catFact.GenreId;
            factText = catFact.FactText;
            sourceId = catFact.SourceId;
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
            if (sourceId > 0) media = _mediaService.GetEntryById(sourceId);
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