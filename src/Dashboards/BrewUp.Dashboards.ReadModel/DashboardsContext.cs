using BrewUp.Dashboards.Entities.Dtos;
using BrewUp.Dashboards.ReadModel.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BrewUp.Dashboards.ReadModel;

public class DashboardsContext(DbContextOptions<DashboardsContext> options) : DbContext(options)
{
    public DbSet<SalesByCustomers> SalesByCustomers { get; set; } = null!;
    public DbSet<MessagesReceived> MessagesReceived { get; set; } = null!;
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Logging configuration
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddFilter((_, level) => level == LogLevel.Information)
                .AddConsole();
        });

        optionsBuilder.UseLoggerFactory(loggerFactory);
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfiguration(new SalesByCustomersMappings());
        modelBuilder.ApplyConfiguration(new SalesByProductsMappings());
        modelBuilder.ApplyConfiguration(new MessagesReceivedMappings());
    }
}