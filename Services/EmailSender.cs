namespace Centers.API.Services;
public sealed class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config ??
            throw new ArgumentNullException(nameof(config));
    }

    public async Task SendEmailAsync(string recipientEmail, string subject, string htmlBody)
    {
        var client = new RestClient(_config[Constants.MailgunSettings.ApiUrl])
        {
            Authenticator = new HttpBasicAuthenticator("api", _config[Constants.MailgunSettings.ApiKey])
        };
        var request = new RestRequest();
        request.AddParameter("domain", _config[Constants.MailgunSettings.SandBoxDomain], ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", _config[Constants.MailgunSettings.From]);
        request.AddParameter("to", recipientEmail);
        request.AddParameter("subject", subject);
        request.AddParameter("html", htmlBody);

        await client.ExecuteAsync(request, Method.Post);
    }

}
