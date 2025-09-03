using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NanoHealthSuite.Data.Enums;
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
        
        builder.Property(e => e.Comments)
            .HasMaxLength(1000);
        
        builder.Property(e => e.UserInputs)
            .HasColumnType("jsonb");

        builder.Property(e => e.Action)
            .IsRequired()
            .HasConversion<EnumToStringConverter<UserAction>>();

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