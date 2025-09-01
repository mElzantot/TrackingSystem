using Microsoft.EntityFrameworkCore;
using NanoHealthSuite.Data.EntitiesConfigurations;
using NanoHealthSuite.Data.Models;

namespace WorkflowTracking.Infrastructure.Data
{
    public class WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowStep> WorkflowSteps { get; set; }
        public DbSet<CustomValidation> CustomValidations { get; set; }
        public DbSet<Process> Processes { get; set; }
        public DbSet<ProcessExecution> ProcessExecutions { get; set; }
        public DbSet<ValidationLog> ValidationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowStepConfiguration());
            modelBuilder.ApplyConfiguration(new CustomValidationConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessConfiguration());
            modelBuilder.ApplyConfiguration(new ProcessExecutionConfiguration());
            modelBuilder.ApplyConfiguration(new ValidationLogConfiguration());
            base.OnModelCreating(modelBuilder);
        }
        
    }
}