namespace NanoHealthSuite.TrackingSystem.Processors;

public interface IHashingService
{
    string Hash(string text);
    bool HashCheck(string hashed, string text);
}