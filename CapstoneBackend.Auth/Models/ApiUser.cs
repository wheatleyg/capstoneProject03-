namespace CapstoneBackend.Auth.Models;

// This is the user data that should be used with the api.
public class ApiUser
{
    public int? Id { get; set; } = null;
    public DateTime CreateDatetime { get; set; } = DateTime.UtcNow;
    public string Username { get; set; } = "";
    public string EmailAddress { get; set; } = "";
    public string Password { get; set; } = "";
    public bool IsDeleted { get; set; }
}