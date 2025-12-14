namespace CapstoneBackend.Core.Models;

// DTOs for joined query results from stored procedures

public class FactTagsWithGenre
{
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string AvailableTagsJson { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool Visible { get; set; }

    // Helper property to deserialize AvailableTags
    public List<string> AvailableTags => string.IsNullOrEmpty(AvailableTagsJson)
        ? new List<string>()
        : System.Text.Json.JsonSerializer.Deserialize<List<string>>(AvailableTagsJson) ?? new List<string>();
}

public class SpaceFactsWithDetails
{
    // SpaceDb fields
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string FactText { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Main table fields
    public string GenreName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool Visible { get; set; }

    // Media table fields
    public MediaType MediaType { get; set; }
    public string MediaLink { get; set; } = string.Empty;
    public DateTime? MediaCreatedAt { get; set; }
    public DateTime? MediaUpdatedAt { get; set; }

    // Helper property to create Media object
    public Media? Media => string.IsNullOrEmpty(MediaLink) ? null : new Media
    {
        Id = SourceId,
        MediaType = MediaType,
        Link = MediaLink,
        CreatedAt = MediaCreatedAt,
        UpdatedAt = MediaUpdatedAt
    };
}

public class CatFactsWithDetails
{
    // CatDb fields
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string FactText { get; set; } = string.Empty;
    public int SourceId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Main table fields
    public string GenreName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public bool Visible { get; set; }

    // Media table fields
    public MediaType MediaType { get; set; }
    public string MediaLink { get; set; } = string.Empty;
    public DateTime? MediaCreatedAt { get; set; }
    public DateTime? MediaUpdatedAt { get; set; }

    // Helper property to create Media object
    public Media? Media => string.IsNullOrEmpty(MediaLink) ? null : new Media
    {
        Id = SourceId,
        MediaType = MediaType,
        Link = MediaLink,
        CreatedAt = MediaCreatedAt,
        UpdatedAt = MediaUpdatedAt
    };
}