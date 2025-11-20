namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;

[Table("cat_db")]
public class CatDb
{
    [Key]
    public int Id { get; set; }
    public int GenreId { get; set; }
    public required string FactText { get; set; }
    public int SourceId { get; set; }
    public DateTime CreatedAt { get; set; } //I wish there was a way to easily test if this linked properly.
}