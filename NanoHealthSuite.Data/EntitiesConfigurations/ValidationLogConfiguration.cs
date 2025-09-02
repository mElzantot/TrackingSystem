using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class ValidationLogConfiguration : IEntityTypeConfiguration<ValidationLog>
{
    public void Configure(EntityTypeBuilder<ValidationLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.IsSuccess)
            .IsRequired();

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(1000);
        
        builder.HasOne(e => e.ProcessExecution)
            .WithMany(pe => pe.ValidationLogs)
            .HasForeignKey(e => e.ProcessExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}