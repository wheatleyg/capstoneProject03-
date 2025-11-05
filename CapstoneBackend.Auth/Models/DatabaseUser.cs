using System.Runtime.CompilerServices;
using Dapper.Contrib.Extensions;

[assembly:InternalsVisibleTo("CapstoneBackend.Test")]

namespace CapstoneBackend.Auth.Models;

//This is the user data that should be used with the database.
[Table("Users")]
internal class DatabaseUser
{
    [Key]
    public int Id { get; set; }
    public DateTime CreateDatetime { get; set; }
    public string Username { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public byte[]? PasswordHash { get; set; }
    public byte[]? PasswordSalt { get; set; }
    public bool IsDeleted { get; set; }
}