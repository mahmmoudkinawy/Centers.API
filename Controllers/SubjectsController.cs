namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/subjects")]
[ApiController]
[ApiVersion("1.0")]
public sealed class SubjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Get paginated subjects endpoint to list the subjects.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the subjects</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /subjects
    ///     {
    ///         "id": "c29638cb-d0eb-4c9e-0518-08db45a6cc76",
    ///         "name": "Math",
    ///         "description": "Math Description"
    ///     },    
    ///     {
    ///         "id": "c29638cb-d0eb-4c9e-0518-08db45a6cc76",
    ///         "name": "NLP",
    ///         "description": "NLP Description"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the all the subjects.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSubjects(
        [FromQuery] SubjectParams subjectParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetSubjectsProcess.Request
            {
                Keyword = subjectParams.Keyword,
                PageNumber = subjectParams.PageNumber,
                PageSize = subjectParams.PageSize
            }, cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    /// <summary>
    /// Create a subject endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /subjects
    ///     {
    ///         "name": "Physics",
    ///         "description": "Physics Description"
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
    public async Task<IActionResult> CreateSubject(
        [FromBody] CreateSubjectProcess.Request request,
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
    /// Updated a subject endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /subjects
    ///     {
    ///         "name": "Physics updated",
    ///         "description": "Physics Description updated"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPut("{subjectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSubject(
        [FromBody] UpdateSubjectProcess.Request request,
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
    /// Remove a subject endpoint by subject id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /subjects/c29638cb-d0eb-4c9e-0518-08db45a6cc76
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpDelete("{subjectId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveSubject(
        [FromRoute] Guid subjectId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveSubjectProcess.Request
            {
                SubjectId = subjectId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }
}
