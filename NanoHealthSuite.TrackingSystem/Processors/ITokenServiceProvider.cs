using NanoHealthSuite.Data.Models;

namespace NanoHealthSuite.TrackingSystem.Processors;

public interface ITokenServiceProvider
{
    AuthResponseDto GenerateAccessToken(User user);
}