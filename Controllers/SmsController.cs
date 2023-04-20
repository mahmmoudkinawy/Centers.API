using Twilio.Types;

namespace Centers.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;

    public SmsController(ISmsService smsService)
    {
        _smsService = smsService;
    }

    [HttpPost]
    public async Task<IActionResult> Send(
        [FromBody] Request request)
    {
        var result = await _smsService.SendSmsAsync(request.PhoneNumber, request.Body);

        if (!string.IsNullOrEmpty(result.ErrorMessage))
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result);
    }
}

public class Request
{
    public string PhoneNumber { get; set; }
    public string Body { get; set; }
}
