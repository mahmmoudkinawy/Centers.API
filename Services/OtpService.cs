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

    // Will be replaced later on.
    public async Task<string> GenerateOtpAsync(
        int digits = 6,
        int timeInMinutes = 60)
    {
        var totp = new Totp(Base32Encoding.ToBytes(_secretKey), totpSize: digits);

        return await Task.FromResult(totp.ComputeTotp(DateTime.UtcNow.AddMinutes(timeInMinutes)));
    }

    public async Task<bool> ValidateOtpAsync(string phoneNumber, string otp)
    {
        return await _context.Otps
            .AnyAsync(o => o.PhoneNumber.Equals(phoneNumber) &&
                           o.Otp.Equals(otp));
    }

    public async Task RemoveOtp(OtpEntity otp)
    {
        _context.Otps.Remove(otp);
        await _context.SaveChangesAsync();
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

    public async Task<OtpEntity> GetOtpByPhoneNumberAsync(string phoneNumber)
    {
        return await _context.Otps.FirstOrDefaultAsync(p => p.PhoneNumber.Equals(phoneNumber));
    }
}
