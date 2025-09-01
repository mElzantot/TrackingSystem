using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.EntitiesConfigurations;

public class ProcessExecutionConfiguration : IEntityTypeConfiguration<ProcessExecution>
{
    public void Configure(EntityTypeBuilder<ProcessExecution> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.ExecutedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(e => e.Comments)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Process)
            .WithMany(p => p.Executions)
            .HasForeignKey(e => e.ProcessId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.WorkflowStep)
            .WithMany(s => s.ProcessExecutions)
            .HasForeignKey(e => e.StepId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.User)
            .WithMany(e => e.ProcessExecutions)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}