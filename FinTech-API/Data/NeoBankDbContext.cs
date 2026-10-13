using Microsoft.EntityFrameworkCore;
using NeoBankApi.Domain.Entities;

namespace NeoBankApi.Data;

public class NeoBankDbContext : DbContext
{
    public NeoBankDbContext(DbContextOptions<NeoBankDbContext> options) : base(options)
    {
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<TransferRecord> TransferRecords => Set<TransferRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>()
            .HasIndex(x => x.Iban)
            .IsUnique();

        modelBuilder.Entity<TransferRecord>()
            .HasIndex(x => x.IdempotencyKey)
            .IsUnique();
    }
}
