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

    [HttpGet]
    public async Task<IActionResult> GetShifts(
        [FromQuery] ShiftParams shiftParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new CetShiftsProcess.Request
            {
                Keyword = shiftParams.Keyword,
                PageNumber = shiftParams.PageNumber,
                PageSize = shiftParams.PageSize
            }, cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    [HttpGet("{shiftId}")]
    public async Task<IActionResult> GetShift(
       [FromRoute] Guid shiftId,
       CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new CetShiftByIdProcess.Request
            {
                ShiftId = shiftId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
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


    [HttpPut("{shiftId:guid}")]
    public async Task<IActionResult> UpdateShift(
        [FromBody] UpdateShiftProcess.Request request,
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


    [HttpDelete("{shiftId}")]
    public async Task<IActionResult> RemoveShift(
        [FromRoute] Guid shiftId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveShiftProcess.Request
            {
                ShiftId = shiftId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return NoContent();
    }

}
