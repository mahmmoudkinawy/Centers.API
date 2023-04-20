namespace Centers.API.Services;
public sealed class SmsService : ISmsService
{
    private readonly TwilioSettings _twilioSettings;

    public SmsService(IOptions<TwilioSettings> twilioSettings)
    {
        _twilioSettings = twilioSettings.Value ??
            throw new ArgumentNullException(nameof(twilioSettings));
    }

    public async Task<MessageResource> SendSmsAsync(string to, string message)
    {
        TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);

        var result = await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_twilioSettings.PhoneNumber),
                to: to
            );

        return result;
    }

}
