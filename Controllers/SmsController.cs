using Twilio.Types;

namespace Centers.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class SmsController : ControllerBase
{
    private readonly ISmsService _smsService;
    private readonly IOtpService _otpService;

    public SmsController(ISmsService smsService, IOtpService otpService)
    {
        _smsService = smsService;
        _otpService = otpService;
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

        var d = await _otpService.GenerateOtpAsync();

        //var va = _otpService.ValidateOtpAsync(d);

        return Ok(result);
    }
}

public class Request
{
    public string PhoneNumber { get; set; }
    public string Body { get; set; }
}
