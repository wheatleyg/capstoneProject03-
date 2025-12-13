namespace CapstoneBackend.Core.Models;

public class FactResponse
{
    public int Id { get; set; }
    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public string FactText { get; set; } = string.Empty;
    public Media? Media { get; set; }
    public string SourceTable { get; set; } = string.Empty;
}