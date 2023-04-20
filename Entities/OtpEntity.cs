namespace Centers.API.Entities;
public sealed class OtpEntity
{
    public Guid Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Otp { get; set; }
    public DateTime? Timestamp { get; set; }
}
