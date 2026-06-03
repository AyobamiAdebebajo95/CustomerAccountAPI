namespace CustomerAccountAPI.Infrastructure.Data;

using CustomerAccountAPI.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<Customer>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Customer>(e =>
        {
            e.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            e.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            e.Property(c => c.DateOfBirth).IsRequired();
            e.Property(c => c.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        });

        builder.Entity<Account>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.AccountNumber).IsRequired().HasMaxLength(20);
            e.HasIndex(a => a.AccountNumber).IsUnique();
            e.Property(a => a.Balance).IsRequired().HasColumnType("decimal(18,2)");
            e.Property(a => a.AccountType).IsRequired().HasMaxLength(50);
            e.Property(a => a.OpenedDate).IsRequired().HasDefaultValueSql("GETUTCDATE()");
            e.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
            e.Property(a => a.CustomerId).IsRequired();
            e.HasOne(a => a.Customer).WithMany(c => c.Accounts)
                .HasForeignKey(a => a.CustomerId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
