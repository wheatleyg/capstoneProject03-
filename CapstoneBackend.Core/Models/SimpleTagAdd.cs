namespace CapstoneBackend.Core.Models;

public class SimpleTagAdd
{
    public required string GenreName { get; set; }
    public required List<string> TagsToAdd { get; set; }
}

