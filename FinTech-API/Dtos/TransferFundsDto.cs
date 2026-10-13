namespace NeoBankApi.Dtos;

public class TransferFundsDto
{
    public string FromIban { get; set; } = string.Empty;
    public string ToIban { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
    public string? Description { get; set; }
}
