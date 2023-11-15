using Microsoft.EntityFrameworkCore;
using JetStreamAPI.Models;

public class JetStreamBackendConfiguration : DbContext
{
    public JetStreamBackendConfiguration(DbContextOptions<JetStreamBackendConfiguration> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configuration for employee table
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired();
        });

    }
}
