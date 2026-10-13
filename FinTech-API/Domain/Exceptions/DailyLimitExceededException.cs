using System.Net;

namespace NeoBankApi.Domain.Exceptions;

public class DailyLimitExceededException : BaseException
{
    public DailyLimitExceededException()
        : base("Перевищено денний ліміт.", HttpStatusCode.Conflict, "daily_limit_exceeded")
    {
    }
}
