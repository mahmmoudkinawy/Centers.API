using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Centers.API.Services;

public class SmsService : ISmsService
{
    private readonly TwilioSettings _twilioSettings;

    public SmsService(IOptions<TwilioSettings> twilioSettings)
    {
        _twilioSettings = twilioSettings.Value;
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
