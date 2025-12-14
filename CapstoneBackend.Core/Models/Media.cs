namespace CapstoneBackend.Core.Models;

using Dapper.Contrib.Extensions;

[Table("media")]
public class Media
{
    [Key]
    public int Id { get; set; }
    public required MediaType MediaType { get; set; }
    public required string Link { get; set; }
    public DateTime? CreatedAt { get; set; }
    [Write(false)]
    public DateTime? UpdatedAt { get; set; }
}

public enum MediaType
{
    Image = 0,
    Video = 1,
    Audio = 2,
    Misc = 3,
    None = 4
}



// My brain now demands I get rid of every error, warning, and 