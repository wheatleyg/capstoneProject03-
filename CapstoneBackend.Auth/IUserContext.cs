namespace CapstoneBackend.Auth;

public interface IUserContext
{
    public bool IsAuthenticated();
    public int GetUserId();
}