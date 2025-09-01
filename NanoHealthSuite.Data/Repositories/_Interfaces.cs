using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.Data.Enums;

public interface IUserRepository : IBaseRepository<User, Guid>;
public interface IUserRoleRepository : IBaseRepository<UserRole, Guid>;

public interface IWorkflowRepository : IBaseRepository<Workflow, Guid>;

public interface IWorkflowStepRepository : IBaseRepository<WorkflowStep, int>;

public interface IProcessRepository : IBaseRepository<Process, int>;

public interface IProcessExecutionRepository : IBaseRepository<ProcessExecution, int>;

public interface ICustomValidationRepository : IBaseRepository<CustomValidation, int>;

public interface IValidationLogRepository : IBaseRepository<ValidationLog, int>;
