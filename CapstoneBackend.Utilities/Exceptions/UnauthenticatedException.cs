using System.Net;

namespace CapstoneBackend.Utilities.Exceptions;

public class UnauthenticatedException : BaseCapstoneException
{
    public UnauthenticatedException(string message, Exception? innerException = null) : base(message, innerException) { }
    public override HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.Unauthorized;
    public override string ClientMessage { get; set; } = "A valid authorization token is required to access this resource.";
}