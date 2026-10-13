using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public class IdempotencyConflictException : BaseException
{
    public IdempotencyConflictException()
        : base("Транзакція з таким ключем вже оброблена.", HttpStatusCode.Conflict, "idempotency_conflict")
    {
    }
}
