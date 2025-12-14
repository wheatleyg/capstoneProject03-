namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;


[Dapper.Contrib.Extensions.Table("fact_tags")]
public class FactTags
{
    [Key]
    public int Id { get; set; }
    public int GenreId { get; set; }

    public List<string> AvailableTags { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Write(false)]
    public DateTime? UpdatedAt { get; set; }

}