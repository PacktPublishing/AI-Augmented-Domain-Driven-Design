using BrewUp.Dashboards.Entities.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BrewUp.Dashboards.ReadModel.Mappings;

public class MessagesReceivedMappings : IEntityTypeConfiguration<MessagesReceived>
{
    public void Configure(EntityTypeBuilder<MessagesReceived> builder)
    {
        builder.ToTable("MessagesReceived", "dbo");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasColumnName("Id")
            .IsRequired()
            .HasMaxLength(36);
        
        builder.Property(x => x.EntityName)
            .HasColumnName("EntityName")
            .IsRequired();
        
        builder.Property(x => x.ReceivedAt)
            .HasColumnName("ReceivedAt")
            .IsRequired();
    }
}