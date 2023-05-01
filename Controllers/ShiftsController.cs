namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/shifts")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
public sealed class ShiftsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShiftsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    public async Task<IActionResult> CreateShift(
        [FromBody] CreateShiftProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

}
