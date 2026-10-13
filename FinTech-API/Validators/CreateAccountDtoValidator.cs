using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NeoBankApi.Data;
using NeoBankApi.Dtos;

namespace NeoBankApi.Validators;

public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    private static readonly string[] AllowedCurrencies = ["UAH", "USD", "EUR"];

    public CreateAccountDtoValidator(NeoBankDbContext dbContext)
    {
        RuleFor(x => x.AccountHolder)
            .NotEmpty().WithMessage("Вкажіть ПІБ власника рахунку.")
            .Length(3, 100).WithMessage("ПІБ має бути від 3 до 100 символів.")
            .Matches(@"^[\p{L}\s-]+$").WithMessage("ПІБ може містити тільки літери, пробіли та дефіси.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий.")
            .EmailAddress().WithMessage("Email має неправильний формат.");

        RuleFor(x => x.Iban)
            .NotEmpty().WithMessage("IBAN обов'язковий.")
            .Matches(@"^UA\d{27}$").WithMessage("IBAN має бути у форматі UA + 27 цифр.")
            .MustAsync(async (iban, cancellationToken) =>
                !await dbContext.BankAccounts.AnyAsync(x => x.Iban == iban, cancellationToken))
            .WithMessage("Рахунок з таким IBAN вже існує.");

        RuleFor(x => x.InitialDeposit)
            .GreaterThanOrEqualTo(0m).WithMessage("Початковий депозит не може бути від'ємним.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта обов'язкова.")
            .Must(x => AllowedCurrencies.Contains(x?.ToUpperInvariant()))
            .WithMessage("Дозволені валюти: UAH, USD, EUR.");

        RuleFor(x => x.DailyLimit)
            .GreaterThan(0m).WithMessage("Денний ліміт має бути більше 0.")
            .LessThanOrEqualTo(200_000m).WithMessage("Денний ліміт не може бути більше 200000.");
    }
}
