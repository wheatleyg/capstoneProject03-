namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;

[Table("space_db")]
public class SpaceDb
{
 [Key]
 public int Id { get; set; }
 public int GenreId { get; set; }
 public required string FactText { get; set; }
 public int SourceId { get; set; }
 public DateTime CreatedAt { get; set; }
}