using System.Net;

namespace CapstoneBackend.Utilities.Exceptions;

public class BadRequestException : BaseCapstoneException
{
    public BadRequestException(string message, Exception? innerException = null) : base(message, innerException) { }

    public override HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.BadRequest;
    public override string ClientMessage { get; set; } = "There was an error with the request. Please look for errors and try again.";
}