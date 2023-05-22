namespace ExamDates.API.Controllers;

[Route("api/v{version:apiVersion}/examDates")]
[ApiController]
[ApiVersion("1.0")]
public sealed class ExamDatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExamDatesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get paginated exam dates endpoint to list the exam dates.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the exam dates</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /examDates
    /// </remarks>
    /// <response code="200">Returns the all the exam dates.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetExamDates(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetExamDatesProcess.Request
            {
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            }, cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }


    /// <summary>
    /// Get exam date by id endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the exam date</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /examDates/5e7c6724-a3e4-4916-ff73-08db45b55673
    /// </remarks>
    /// <response code="200">Returns the matched exam date with the given id.</response>
    /// <response code="401">User does not exist.</response>    
    /// <response code="404">No exam date matches this id.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize]
    [HttpGet("{examDateId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetExamDateById(
        [FromRoute] Guid examDateId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetExamDateByIdProcess.Request
            {
                ExamDateId = examDateId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
    }


    /// <summary>
    /// Create a exam date endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /examDates
    ///     {
    ///         "date":"2023-05-08T09:26:59.123Z",
    ///         "openingDate":"2023-06-08T09:26:59.123Z",
    ///         "closingDate":"2023-07-08T09:26:59.123Z"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateExamDate(
        [FromBody] CreateExamDateProcess.Request request,
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
    /// Create an exam date endpoint with the center admin.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /32ac4612-3721-4428-85ec-d6606e548d15/book-exam-date
    ///     { }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeCenterAdmin)]
    [HttpPost("{examDateId}/book-exam-date")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BookExamDate(
        [FromRoute] Guid examDateId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new ExamDateBookingByCenterAdminProcess.Request
            {
                ExamDateId = examDateId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }


    /// <summary>
    /// Updated a exam date endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /examDates/d36e0fda-47f3-4ace-ff72-08db45b55673
    ///     {
    ///         "date":"2024-05-08T09:26:59.123Z",
    ///         "openingDate":"2024-06-08T09:26:59.123Z",
    ///         "closingDate":"2024-07-08T09:26:59.123Z"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPut("{examDateId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateExamDate(
        [FromBody] UpdateExamDateProcess.Request request,
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
    /// Remove an Exam Date endpoint by Exam Date id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /examDates/c29638cb-d0eb-4c9e-0518-08db45a6cc76
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpDelete("{examDateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveExamDate(
        [FromRoute] Guid examDateId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveExamDateProcess.Request
            {
                ExamDateId = examDateId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
}
