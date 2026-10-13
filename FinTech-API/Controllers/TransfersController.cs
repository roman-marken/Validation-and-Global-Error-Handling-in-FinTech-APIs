using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NeoBankApi.Data;
using NeoBankApi.Domain.Entities;
using NeoBankApi.Domain.Exceptions;
using NeoBankApi.Dtos;

namespace NeoBankApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly NeoBankDbContext _dbContext;
    private readonly IValidator<TransferFundsDto> _validator;

    public TransfersController(NeoBankDbContext dbContext, IValidator<TransferFundsDto> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Transfer(TransferFundsDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new AppValidationException(validationResult.Errors);
        }

        var alreadyProcessed = await _dbContext.TransferRecords
            .AnyAsync(x => x.IdempotencyKey == dto.IdempotencyKey && x.Status == "Completed", cancellationToken);

        if (alreadyProcessed)
        {
            throw new IdempotencyConflictException();
        }

        var fromAccount = await _dbContext.BankAccounts
            .FirstOrDefaultAsync(x => x.Iban == dto.FromIban, cancellationToken);

        var toAccount = await _dbContext.BankAccounts
            .FirstOrDefaultAsync(x => x.Iban == dto.ToIban, cancellationToken);

        if (fromAccount is null)
        {
            throw new NotFoundException("Рахунок відправника", dto.FromIban);
        }

        if (toAccount is null)
        {
            throw new NotFoundException("Рахунок отримувача", dto.ToIban);
        }

        if (!fromAccount.IsActive)
        {
            throw new AccountBlockedException(fromAccount.Iban);
        }

        if (!toAccount.IsActive)
        {
            throw new AccountBlockedException(toAccount.Iban);
        }

        if (fromAccount.Balance < dto.Amount)
        {
            throw new InsufficientFundsException();
        }

        if (dto.Amount > fromAccount.DailyLimit)
        {
            throw new DailyLimitExceededException();
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        fromAccount.Balance -= dto.Amount;
        toAccount.Balance += dto.Amount;

        var transferRecord = new TransferRecord
        {
            FromIban = dto.FromIban,
            ToIban = dto.ToIban,
            Amount = dto.Amount,
            Currency = fromAccount.Currency,
            Description = dto.Description ?? string.Empty,
            IdempotencyKey = dto.IdempotencyKey,
            Status = "Completed",
            ProcessedAt = DateTime.UtcNow
        };

        _dbContext.TransferRecords.Add(transferRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Created($"/api/transfers/{transferRecord.Id}", transferRecord);
    }
}
