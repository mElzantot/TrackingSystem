using NanoHealthSuite.Data.Enums;

namespace NanoHealthSuite.TrackingSystem.Services;

public class ProcessHelper
{
    public bool IsApprovedAction(UserAction action)
    {
        return action is UserAction.Approve or UserAction.Submit;
    }

}