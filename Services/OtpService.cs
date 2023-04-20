namespace Centers.API.Services;
public sealed class OtpService : IOtpService
{
    private readonly CentersDbContext _context;
    private readonly string _secretKey;

    public OtpService(CentersDbContext context, IConfiguration config)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _secretKey = config.GetValue<string>("OtpSecretKey")!;
    }

    public async Task<string> GenerateOtpAsync(
        int digits = 6,
        int timeInMinutes = 60)
    {
        var totp = new Totp(Base32Encoding.ToBytes(_secretKey), totpSize: digits);

        return await Task.FromResult(totp.ComputeTotp(DateTime.UtcNow.AddMinutes(timeInMinutes)));
    }

    public async Task<bool> ValidateOtpAsync(string phoneNumber, string otp)
    {
        var totp = new Totp(Base32Encoding.ToBytes(_secretKey));

        var verify = totp.VerifyTotp(otp, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

        if (verify)
        {
            var otpEntity = await _context.Otps.FirstOrDefaultAsync(o => o.PhoneNumber.Equals(phoneNumber));
            _context.Otps.Remove(otpEntity);
            await _context.SaveChangesAsync();

            return await Task.FromResult(verify);
        }

        return await Task.FromResult(verify);
    }

    public async Task StoreOtp(string phoneNumber, string otp)
    {
        var otpEntity = new OtpEntity
        {
            Id = Guid.NewGuid(),
            Otp = otp,
            PhoneNumber = phoneNumber,
            Timestamp = DateTime.UtcNow
        };

        _context.Otps.Add(otpEntity);
        await _context.SaveChangesAsync();
    }

}
