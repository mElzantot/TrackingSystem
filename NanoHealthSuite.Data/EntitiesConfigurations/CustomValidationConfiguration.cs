using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class CustomValidationConfiguration : IEntityTypeConfiguration<CustomValidation>
{
    public void Configure(EntityTypeBuilder<CustomValidation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ValidationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.Data)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.HasOne(e => e.Step)
            .WithMany(s => s.Validations)
            .HasForeignKey(e => e.StepId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}