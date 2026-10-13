namespace NeoBankApi.Domain.Entities;

public class BankAccount
{
    public int Id { get; set; }
    public string Iban { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = "UAH";
    public decimal DailyLimit { get; set; } = 50_000m;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
