using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public class AccountBlockedException : BaseException
{
    public AccountBlockedException(string iban)
        : base($"Рахунок '{iban}' заблоковано.", HttpStatusCode.Conflict, "account_blocked")
    {
    }
}
