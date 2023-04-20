namespace Centers.API.Services;
public interface IOtpService
{
    Task<string> GenerateOtpAsync(int digits = 6, int timeInMinutes = 60);
    Task<bool> ValidateOtpAsync(string phoneNumber, string otp);
    Task<OtpEntity> GetOtpByPhoneNumberAsync(string phoneNumber);
    Task StoreOtp(string phoneNumber, string otp);
    Task RemoveOtp(OtpEntity otp);
}
