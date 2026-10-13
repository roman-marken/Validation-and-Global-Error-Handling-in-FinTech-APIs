using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public class InsufficientFundsException : BaseException
{
    public InsufficientFundsException()
        : base("Недостатньо коштів на рахунку.", HttpStatusCode.Conflict, "insufficient_funds")
    {
    }
}
