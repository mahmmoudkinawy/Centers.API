﻿namespace Centers.API.Controllers;

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
    /// Get paginated centers endpoint by exam date.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the centers</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers/filter-by-exam-date?examDate=2023-05-08T09:26:59.123Z
    /// </remarks>
    /// <response code="200">Returns the all the centers.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet("filter-by-exam-date")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCentersByExamDate(
        [FromQuery] ExamDateParams examDateParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCentersByExamDateProcess.Request
            {
                PageNumber = examDateParams.PageNumber,
                PageSize = examDateParams.PageSize,
                ExamDate = examDateParams.ExamDate
            }, cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }


    /// <summary>
    /// Get paginated center admins endpoint to list the center admins.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the center admins</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers/admins
    /// </remarks>
    /// <response code="200">Returns the all center admins.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet("admins")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCenterAdmins(
        [FromQuery] CenterParams centerParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCenterAdminsProcess.Request
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
    /// Get center with shifts by id endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the center with shifts</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers/5e7c6724-a3e4-4916-ff73-08db45b55673/with-shifts
    /// </remarks>
    /// <response code="200">Returns the center with shifts via the given id.</response>
    /// <response code="401">User does not exist.</response>    
    /// <response code="404">No center matches this id.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet("{centerId}/with-shifts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCenterWithShiftsById(
        [FromRoute] Guid centerId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCenterWithShiftsByCenterIdProcess.Request
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
    /// Get center by id endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the center</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /centers/5e7c6724-a3e4-4916-ff73-08db45b55673
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
    ///         "name": "Center",
    ///         "gender": "Male",
    ///         "zone": "Ras al-Khaimah",
    ///         "locationUrl": "https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1001.jpg",
    ///         "capacity": 50,
    ///         "isEnabled": true,
    ///         "ownerId": "340b60b6-4b03-48cc-848a-0bb896f77842"
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
    ///         "name": "Center update",
    ///         "gender": "Female",
    ///         "zone": "Ras al-Khaimah",
    ///         "locationUrl": "https://cloudflare-ipfs.com/ipfs/Qmd3W5DuhgHirLHGVixi6V76LhCkZUz6pnFt5AJBiyvHye/avatar/1001.jpg",
    ///         "capacity": 25,
    ///         "isEnabled": false,
    ///         "ownerId": "622ac7e5-66e0-45e0-9496-7c7b7f7774ad"
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
