using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(e => e.UserRole)
            .WithMany()
            .HasForeignKey(e => e.UserRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(e => e.ActionType)
            .IsRequired()
            .HasConversion<EnumToStringConverter<ActionType>>();

        builder.HasOne(e => e.Workflow)
            .WithMany(w => w.Steps)
            .HasForeignKey(e => e.WorkflowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.NextStep)
            .WithOne(p => p.PreviousStepOf)
            .HasForeignKey<WorkflowStep>(p => p.NextStepId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
