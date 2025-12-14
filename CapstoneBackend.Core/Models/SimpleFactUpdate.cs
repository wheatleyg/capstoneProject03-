namespace CapstoneBackend.Core.Models;

public class SimpleFactUpdate
{
    public required int FactId { get; set; }
    public required string GenreName { get; set; }
    public string? FactText { get; set; }
    public string? MediaLink { get; set; }
    public MediaType? MediaType { get; set; }
}




