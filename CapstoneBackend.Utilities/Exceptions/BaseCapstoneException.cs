using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneBackend.Utilities.Exceptions;

public abstract class BaseCapstoneException : Exception
{
    protected BaseCapstoneException(string message, Exception? innerException = null) : base(message, innerException) {

    }

    public abstract HttpStatusCode HttpStatusCode { get; set; }
    public int HttpCode => (int) HttpStatusCode;
    public virtual string ClientMessage { get; set; } = "An error occured while processing the request. Please try again.";

    public IActionResult ReturnDefaultResponse()
    {
        {
            return new ObjectResult(new { message = this.ClientMessage })
            {
                StatusCode = this.HttpCode
            };
        }
    }
}