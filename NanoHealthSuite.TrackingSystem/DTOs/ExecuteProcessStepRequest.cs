using NanoHealthSuite.Data.Enums;
using NanoHealthSuite.TrackingSystem.DTOs;

namespace NanoHealthSuite.TrackingSystem;

public class ExecuteProcessStepRequest
{
    public int ProcessId { get; set; }
    public string StepName { get; set; }
    public UserAction Action { get; set; }
    public string Comment { get; set; }
    public List<UserInput>? UserInputs { get; set; }
}