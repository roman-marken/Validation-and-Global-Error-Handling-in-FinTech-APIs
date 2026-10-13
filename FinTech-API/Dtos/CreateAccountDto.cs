namespace NeoBankApi.Dtos;

public class CreateAccountDto
{
    public string Iban { get; set; } = string.Empty;
    public string AccountHolder { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal InitialDeposit { get; set; }
    public string Currency { get; set; } = "UAH";
    public decimal DailyLimit { get; set; } = 50_000m;
}
