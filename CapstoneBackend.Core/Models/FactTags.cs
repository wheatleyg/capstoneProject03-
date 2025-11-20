namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations.Schema;


[Dapper.Contrib.Extensions.Table("fact_tags")]
public class FactTags
{
    [Key]
    public int FactId { get; set; }

    [Column("fact_tags")] public required string Tags { get; set; } = "[]"; //another foresight by myself.

}