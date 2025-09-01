using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class ProcessConfiguration : IEntityTypeConfiguration<Process>
{
    public void Configure(EntityTypeBuilder<Process> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.StartedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Workflow)
            .WithMany(w => w.Processes)
            .HasForeignKey(e => e.WorkflowId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CurrentStep)
            .WithMany(e => e.Processes)
            .HasForeignKey(e => e.CurrentStepId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.Initiator)
            .WithMany(e => e.InitiatedProcesses)
            .HasForeignKey(e => e.InitiatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}