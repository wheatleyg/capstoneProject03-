namespace CapstoneBackend.Core.Models;

public class SimpleFactDelete
{
    public required int FactId { get; set; }
    public required string GenreName { get; set; }
    public bool DeleteMedia { get; set; } = true; // Default to true - delete media if not used elsewhere
}

