using BrewUp.Dashboards.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrewUp.Dashboards.ReadModel.Mappings;

public class SalesByProductsMappings : IEntityTypeConfiguration<SalesByProducts>
{
    public void Configure(EntityTypeBuilder<SalesByProducts> builder)
    {
        builder.ToTable("SalesByProducts", "dbo");
        builder.HasKey(x => new { x.Id, x.Year });
        
        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(36);
        
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