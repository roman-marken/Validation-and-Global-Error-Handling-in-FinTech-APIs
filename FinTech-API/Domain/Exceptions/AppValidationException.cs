using System.Net;
using FluentValidation.Results;

namespace NeoBankApi.Domain.Exceptions;

public class AppValidationException : BaseException
{
    public AppValidationException(IEnumerable<ValidationFailure> failures)
        : base("Помилка валідації.", HttpStatusCode.BadRequest, "validation_error")
    {
        Errors = failures
            .GroupBy(x => x.PropertyName)
            .ToDictionary(x => x.Key, x => x.Select(e => e.ErrorMessage).ToArray());
    }

    public IDictionary<string, string[]> Errors { get; }
}
