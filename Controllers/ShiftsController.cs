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

    /// <summary>
    /// Update shift capacity by center id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT bf2f6872-6f47-4b7d-8079-4f9cb612f3e8/update-shift-capacity-for-center/8be0e1e4-3fc9-4ece-bde8-6a14fc50c139
    ///         {
    ///             "capacity": 4999
    ///         }
    /// </remarks>
    /// <response code="204">Success and returns no content.</response>
    /// <response code="404">No shifts to update.</response>
    /// <response code="404">Validation Errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpPut("{shiftId:guid}/update-shift-capacity-for-center/{centerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateShiftCapacityForCenter(
        [FromBody] UpdateShiftCapacityForCenterProcess.Request request,
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

    /// <summary>
    /// Update shift status for center.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT bf2f6872-6f47-4b7d-8079-4f9cb612f3e8/update-shift-capacity-for-center/8be0e1e4-3fc9-4ece-bde8-6a14fc50c139
    ///         {
    ///             "isEnabled": false
    ///         }
    /// </remarks>
    /// <response code="204">Success and returns no content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    /// <response code="404">No shifts to update.</response>
    [HttpPut("{shiftId:guid}/update-shift-status-for-center/{centerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateShiftStatusForCenter(
        [FromBody] UpdateShiftStatusForCenterProcess.Request request,
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

    /// <summary>
    /// Updates all the shifts time.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /update-shifts-time/
    ///         {
    ///             "shiftStartTime": "2023-05-01T17:58:05.82Z",
    ///             "shiftDuration": "00:30:00
    ///         }
    /// </remarks>
    /// <response code="204">Success and returns no content.</response>
    /// <response code="404">No shifts to update.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpPut("update-shifts-time")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateShiftsTime(
        [FromBody] UpdateShiftTimeProcess.Request request,
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
