namespace NanoHealthSuite.TrackingSystem;

public class AuthResponseDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }

}