using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
{
    public void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.StepName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.AssignedTo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ActionType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

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
