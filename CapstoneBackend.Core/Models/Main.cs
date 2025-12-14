namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;

[Table("main")]
public class Main
{
    [Key]
    public int Id { get; set; }
    public required string GenreName { get; set; }
    public required string Description { get; set; }
    public required string TableName { get; set; }
    public bool Visible { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Write(false)]
    public DateTime? UpdatedAt { get; set; }
}