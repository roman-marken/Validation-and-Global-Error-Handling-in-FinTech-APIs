namespace NeoBankApi.Domain.Entities;

public class TransferRecord
{
    public int Id { get; set; }
    public string FromIban { get; set; } = string.Empty;
    public string ToIban { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "UAH";
    public string Description { get; set; } = string.Empty;
    public string IdempotencyKey { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
