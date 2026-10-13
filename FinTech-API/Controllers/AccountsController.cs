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
public class AccountsController : ControllerBase
{
    private readonly NeoBankDbContext _dbContext;
    private readonly IValidator<CreateAccountDto> _validator;

    public AccountsController(NeoBankDbContext dbContext, IValidator<CreateAccountDto> validator)
    {
        _dbContext = dbContext;
        _validator = validator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAccountDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new AppValidationException(validationResult.Errors);
        }

        var account = new BankAccount
        {
            Iban = dto.Iban,
            AccountHolder = dto.AccountHolder.Trim(),
            Email = dto.Email.Trim(),
            Balance = dto.InitialDeposit,
            Currency = dto.Currency.ToUpperInvariant(),
            DailyLimit = dto.DailyLimit,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.BankAccounts.Add(account);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetByIban), new { iban = account.Iban }, account);
    }

    [HttpGet("{iban}")]
    public async Task<IActionResult> GetByIban(string iban, CancellationToken cancellationToken)
    {
        var account = await _dbContext.BankAccounts
            .FirstOrDefaultAsync(x => x.Iban == iban, cancellationToken);

        if (account is null)
        {
            throw new NotFoundException("Рахунок", iban);
        }

        return Ok(account);
    }
}
