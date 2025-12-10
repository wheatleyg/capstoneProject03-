//ReSharper disable ConvertToAutoPropertyWithPrivateSetter
namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;

[Table("cat_db")]
public class CatDb
{
    [Key]
    public int Id { get; set; }
    public int GenreId { get; init; }
    public required string FactText { get; init; }
    public int SourceId { get; init; }
    public DateTime? CreatedAt { get; set; }
}
//ReSharper restore ConvertToAutoPropertyWithPrivateSetter