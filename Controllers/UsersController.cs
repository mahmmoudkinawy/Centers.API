namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/users")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserProcess.Request request,
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

}
