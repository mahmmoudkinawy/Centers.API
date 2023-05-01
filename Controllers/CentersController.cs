namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/centers")]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
public sealed class CentersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CentersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get paginated centers endpoint to list the centers.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the centers</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers
    ///     {
    ///         "id": "5e7c6724-a3e4-4916-ff73-08db45b55673",
    ///         "name": "Ibn Elhithm",
    ///         "capacity": 5000,
    ///         "description": "Great center",
    ///         "openingDate": "2023-05-01T08:00:00Z",
    ///         "closingDate": "2023-10-01T08:00:00Z"
    ///     },    
    ///     {
    ///         "id": "d36e0fda-47f3-4ace-ff72-08db45b55673",
    ///         "name": "Mohamdy",
    ///         "capacity": 2236,
    ///         "description": "Not bad center as you know",
    ///         "openingDate": "2023-05-01T09:00:00Z",
    ///         "closingDate": "2023-10-01T08:30:00Z"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the all the centers.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCenters(
        [FromQuery] CenterParams centerParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCentersProcess.Request
            {
                Keyword = centerParams.Keyword,
                PageNumber = centerParams.PageNumber,
                PageSize = centerParams.PageSize
            }, cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }


    /// <summary>
    /// Get center by id endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the center</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers/5e7c6724-a3e4-4916-ff73-08db45b55673
    ///     {
    ///         "id": "5e7c6724-a3e4-4916-ff73-08db45b55673",
    ///         "name": "Ibn Elhithm",
    ///         "capacity": 5000,
    ///         "description": "Great center",
    ///         "openingDate": "2023-05-01T08:00:00Z",
    ///         "closingDate": "2023-10-01T08:00:00Z"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the matched center with the given id.</response>
    /// <response code="401">User does not exist.</response>    
    /// <response code="404">No center matches this id.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet("{centerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCenterById(
        [FromRoute] Guid centerId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCenterByIdProcess.Request
            {
                CenterId = centerId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }


    /// <summary>
    /// Create a center endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /centers
    ///     {
    ///         "name": "Ibn Elhithm",
    ///         "capacity": 5000,
    ///         "description": "Great center",
    ///         "openingDate": "2023-05-01T08:00:00Z",
    ///         "closingDate": "2023-10-01T08:00:00Z"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCenter(
        [FromBody] CreateCenterProcess.Request request,
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
    /// Updated a center endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /centers/d36e0fda-47f3-4ace-ff72-08db45b55673
    ///     {
    ///         "name": "Ibn Elhithm updated",
    ///         "capacity": 236,
    ///         "description": "Great center updated",
    ///         "openingDate": "2023-05-01T08:00:00Z",
    ///         "closingDate": "2023-10-01T08:00:00Z"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpPut("{centerId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateCenter(
        [FromBody] UpdateCenterProcess.Request request,
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
    /// Remove a center endpoint by center id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /centers/c29638cb-d0eb-4c9e-0518-08db45a6cc76
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpDelete("{centerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveCenter(
        [FromRoute] Guid centerId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveCenterProcess.Request
            {
                CenterId = centerId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
}
