using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.Data.Models;
using WorkflowTracking.Infrastructure.Data;

namespace WorkflowTracking.Infrastructure.Repositories;

public class UserRoleRepository : BaseRepository<WorkflowDbContext, UserRole, Guid>, IUserRoleRepository
{
    public UserRoleRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class UserRepository : BaseRepository<WorkflowDbContext, User, Guid>, IUserRepository
{
    public UserRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class WorkflowRepository : BaseRepository<WorkflowDbContext, Workflow, Guid>, IWorkflowRepository
{
    public WorkflowRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class WorkflowStepRepository : BaseRepository<WorkflowDbContext, WorkflowStep, int>, IWorkflowStepRepository
{
    public WorkflowStepRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class ProcessRepository : BaseRepository<WorkflowDbContext, Process, int>, IProcessRepository
{
    public ProcessRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class ProcessExecutionRepository : BaseRepository<WorkflowDbContext, ProcessExecution, int>, IProcessExecutionRepository
{
    public ProcessExecutionRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class CustomValidationRepository : BaseRepository<WorkflowDbContext, CustomValidation, int>, ICustomValidationRepository
{
    public CustomValidationRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}

public class ValidationLogRepository : BaseRepository<WorkflowDbContext, ValidationLog, int>, IValidationLogRepository
{
    public ValidationLogRepository(WorkflowDbContext context) : base(context, (e, i) => e.Id = i)
    {
    }
}