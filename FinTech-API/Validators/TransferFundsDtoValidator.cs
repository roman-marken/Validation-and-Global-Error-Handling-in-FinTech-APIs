using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NeoBankApi.Data;
using NeoBankApi.Dtos;

namespace NeoBankApi.Validators;

public class TransferFundsDtoValidator : AbstractValidator<TransferFundsDto>
{
    public TransferFundsDtoValidator(NeoBankDbContext dbContext)
    {
        RuleFor(x => x.FromIban)
            .NotEmpty().WithMessage("IBAN відправника обов'язковий.")
            .Matches(@"^UA\d{27}$").WithMessage("IBAN відправника має бути у форматі UA + 27 цифр.")
            .MustAsync(async (iban, cancellationToken) =>
                await dbContext.BankAccounts.AnyAsync(x => x.Iban == iban, cancellationToken))
            .WithMessage("Рахунок відправника не знайдено.");

        RuleFor(x => x.ToIban)
            .NotEmpty().WithMessage("IBAN отримувача обов'язковий.")
            .Matches(@"^UA\d{27}$").WithMessage("IBAN отримувача має бути у форматі UA + 27 цифр.")
            .MustAsync(async (iban, cancellationToken) =>
                await dbContext.BankAccounts.AnyAsync(x => x.Iban == iban, cancellationToken))
            .WithMessage("Рахунок отримувача не знайдено.");

        RuleFor(x => x)
            .Must(x => x.FromIban != x.ToIban)
            .WithMessage("Не можна переказувати кошти на той самий рахунок.")
            .OverridePropertyName(nameof(TransferFundsDto.ToIban));

        RuleFor(x => x.Amount)
            .GreaterThan(0m).WithMessage("Сума переказу має бути більше 0.")
            .LessThanOrEqualTo(100_000m).WithMessage("Максимальна сума однієї операції - 100000.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("IdempotencyKey обов'язковий.")
            .Length(10, 100).WithMessage("IdempotencyKey має бути від 10 до 100 символів.");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Опис не може бути довше 250 символів.");
    }
}
