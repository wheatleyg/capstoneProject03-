using Dapper.Contrib.Extensions;

namespace CapstoneBackend.Core.Models;

[Table("main")]
public class Main
{
    [Key]
    public int Id { get; set; }
    public required string GenreName { get; set; }
    public required string Description { get; set; }
    public required string TableName { get; set; }
    public bool Visible { get; set; }
}