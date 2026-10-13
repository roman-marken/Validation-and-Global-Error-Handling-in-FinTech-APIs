using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public abstract class BaseException : Exception
{
    protected BaseException(string message, HttpStatusCode statusCode, string errorCode) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }

    public HttpStatusCode StatusCode { get; }
    public string ErrorCode { get; }
}
