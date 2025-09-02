using System.Web.Helpers;

namespace NanoHealthSuite.TrackingSystem.Processors;

public class HashingService : IHashingService
{
    public string Hash(string text)
    {
        return Crypto.HashPassword(text);
    }

    public bool HashCheck(string hashed , string text)
    {
        return Crypto.VerifyHashedPassword(hashed , text);
    }

}