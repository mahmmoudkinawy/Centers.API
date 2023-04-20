namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/account")]
[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
public sealed class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// login endpoint to authenticate the user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the full name with the token as a plain text</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /login
    ///     {
    ///        "email":"bob@test.com",
    ///        "password":"Pa$$w0rd"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the user if exists with full name and token.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }

    /// <summary>
    /// Register endpoint to register the user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the full name with the token as a plain text along with the user image url.</returns>
    /// /// <remarks>
    /// Sample request:
    ///
    ///     POST /register
    ///     Content-Type: multipart/form-data
    ///     
    ///     --boundary
    ///     Content-Disposition: form-data; name="firstName"
    ///
    ///     bob
    ///     --boundary
    ///     Content-Disposition: form-data; name="lastName"
    ///
    ///     bob
    ///     --boundary
    ///     Content-Disposition: form-data; name="gender"
    ///
    ///     Male
    ///     --boundary
    ///     Content-Disposition: form-data; name="phoneNumber"
    ///
    ///     +971501234567
    ///     --boundary
    ///     Content-Disposition: form-data; name="email"
    ///
    ///     bob@test.com
    ///     --boundary
    ///     Content-Disposition: form-data; name="password"
    ///
    ///     Pa$$w0rd
    ///     --boundary
    ///     Content-Disposition: form-data; name="hasDisability"
    ///
    ///     true
    ///     --boundary
    ///     Content-Disposition: form-data; name="disability"; filename="myDisability.jpg"
    ///     Content-Type: image/jpeg
    ///
    ///     --boundary
    ///     Content-Disposition: form-data; name="NationalId";
    ///     
    ///     784-1997-1234567-8
    ///     [binary data]
    ///     --boundary--
    ///
    /// </remarks>
    /// <response code="200">Returns the newly created user full name and token.</response>
    /// <response code="400">If the item is got validation errors.</response>
    /// <response code="401">Unauthorized as email is already taken of something..</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Register(
        [FromForm] UserRegisterProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }


    /// <summary>
    /// Verify Otp endpoint to verify the otp.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns if the operations is successful or not.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /verify-otp
    ///     {
    ///        "phoneNumber":"+201208534244",
    ///        "otpCode":"Pa$$w0rd"
    ///     }
    /// </remarks>
    /// <response code="200">Returns if the operations is successful or not.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    [HttpPost("verify-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> VerifyOtp(
        [FromBody] VerifyOtpByPhoneNumberProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }


}
