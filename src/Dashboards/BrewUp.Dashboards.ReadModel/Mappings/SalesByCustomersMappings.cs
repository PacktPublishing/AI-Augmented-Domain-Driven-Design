using BrewUp.Dashboards.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrewUp.Dashboards.ReadModel.Mappings;

public class SalesByCustomersMappings : IEntityTypeConfiguration<SalesByCustomers>
{
    public void Configure(EntityTypeBuilder<SalesByCustomers> builder)
    {
        builder.ToTable("SalesByCustomers", "dbo");
        builder.HasKey(x => new { x.Id, x.Year });
        
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(x => x.CustomerName)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(x => x.Year)
            .IsRequired()
            .HasMaxLength(4);
        builder.Property(x => x.TotalSales)
            .IsRequired()
            .HasColumnType("decimal(18,2)");
        builder.Property(x => x.Currency)
            .IsRequired()
            .HasMaxLength(5);
    }
}