using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string entityName, string value)
        : base($"{entityName} '{value}' не знайдено.", HttpStatusCode.NotFound, "not_found")
    {
    }
}
