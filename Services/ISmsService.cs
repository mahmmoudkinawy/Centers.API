namespace Centers.API.Services;
public interface ISmsService
{
    Task<MessageResource> SendSmsAsync(string to, string message);
}
