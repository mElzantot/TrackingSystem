using NanoHealthSuite.Data.Models;
using NanoHealthSuite.TrackingSystem.Shared;

namespace NanoHealthSuite.Validation;

public interface IProcessStepValidationStrategy
{
    Task<Result<string>> ValidateAsync(
        Dictionary<string,string> data,
        CustomValidation validation);
}