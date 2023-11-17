using Microsoft.EntityFrameworkCore;
using JetStreamAPI.Services;
using JetStreamAPI.Models;

public class JetStreamBackendConfiguration : DbContext
{
    public JetStreamBackendConfiguration(DbContextOptions<JetStreamBackendConfiguration> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<ServiceOrder> ServiceOrders { get; set; }
    public DbSet<ServiceType> ServiceTypes { get; set; }

protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // configuration for employee table
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).IsRequired();
        });

        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Username = "Arda", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 2, Username = "Satoru", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 3, Username = "Smith", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 4, Username = "Lukas", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 5, Username = "Daniel", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 6, Username = "Tobey", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 7, Username = "Micheal", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 8, Username = "Brian", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 9, Username = "Alim", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 },
            new Employee { Id = 10, Username = "Sven", Password = "1234", IsLocked = false, FailedLoginAttempts = 0 }
        );

        modelBuilder.Entity<ServiceType>().HasData(
            new ServiceType { Id = 1, Name = "Kleiner Service", Cost = 34.95m },
            new ServiceType { Id = 2, Name = "Grosser Service", Cost = 59.95m },
            new ServiceType { Id = 3, Name = "Rennski-Service", Cost = 74.95m },
            new ServiceType { Id = 4, Name = "Bindung montieren und einstellen", Cost = 24.95m },
            new ServiceType { Id = 5, Name = "Fell zuschneiden", Cost = 14.95m },
            new ServiceType { Id = 6, Name = "Heisswachsen", Cost = 19.95m }
        );
    }
}
