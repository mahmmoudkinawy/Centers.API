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
    /// Get paginated shifts.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the shifts along with the subjects.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /shifts?pageNumber=1&Keyword=hello
    ///     [
    ///         {
    ///             "id": "89967524-58c1-4a32-bd17-bfbe573034cc",
    ///             "shiftStartTime": "2023-05-01T17:58:05.82Z",
    ///             "shiftEndTime": "2023-05-03T17:58:05.82Z",
    ///             "centerName": "حقيقة",
    ///             "centerId": "808e2447-bc6e-cc57-e2fb-07aa702a94e0",
    ///             "adminName": "Admin Center 2",
    ///             "adminId": "2fb8d84f-f9da-4ed8-b5d2-2f68a049b1e7",
    ///             "subjects": [
    ///             {
    ///                 "id": "f28ce390-ddc5-42ae-9a80-d91de0a6c62c",
    ///                 "name": "connect",
    ///                 "description": "هناك ظهور الخارجي مستخدماً بمعنى لينتشر المطابع وليس طبيعياَ وعاد. يقرأها ودور ظهور. ويُستخدم أو لهذه. من ولذلك وعاد مؤخراَ أو طبيعياَ. استخدام سيلهي شكل قرون عشر الإلكتروني. التي لأنها ولذلك. ألدوس النص خمسة أيضاً الزمن حقيقة الخارجي وهي."
    ///             },
    ///             {
    ///                 "id": "09be7293-32ab-43a7-aad6-5cb08ceed47d",
    ///                 "name": "back up",
    ///                 "description": "الزمن برص أخذتها إيبسوم المطابع حتى لهذه الصفحة استخدام قرون. بمعنى مجهولة مطبعة الشكلي طبيعياَ زمن الشكلي. المعيار حتى الأصلي نسخ ويُستخدم مرة ليتراسيت. الصفحة البلاستيكية ألدوس خمسة مطبعة النشر دليل منذ مستخدماً بايج."
    ///             }
    ///         },    
    ///         {
    ///             "id": "89967524-58c1-4a32-bd17-bfbe573034cs",
    ///             "shiftStartTime": "2023-06-01T17:58:05.82Z",
    ///             "shiftEndTime": "2023-06-03T17:58:05.82Z",
    ///             "centerName": "لا",
    ///             "centerId": "808e2447-bc6e-cc57-e2fb-07aa702a94e1",
    ///             "adminName": "Admin Center 5",
    ///             "adminId": "2fb8d84f-f9da-4ed8-b5d2-2f68a049b1e1",
    ///             "subjects": [
    ///             {
    ///                 "id": "f28ce390-ddc5-42ae-9a80-d91de0a6c62as",
    ///                 "name": "connect 2",
    ///                 "description": "هناك ظهور الخارجي مستخدماً بمعنى لينتشر المطابع وليس طبيعياَ وعاد. يقرأها ودور ظهور. ويُستخدم أو لهذه. من ولذلك وعاد مؤخراَ أو طبيعياَ. استخدام سيلهي شكل قرون عشر الإلكتروني. التي لأنها ولذلك. ألدوس النص خمسة أيضاً الزمن حقيقة الخارجي وهي."
    ///             },
    ///             {
    ///                 "id": "09be7293-32ab-43a7-aad6-5cb08ceed47s",
    ///                 "name": "back up 2",
    ///                 "description": "الزمن برص أخذتها إيبسوم المطابع حتى لهذه الصفحة استخدام قرون. بمعنى مجهولة مطبعة الشكلي طبيعياَ زمن الشكلي. المعيار حتى الأصلي نسخ ويُستخدم مرة ليتراسيت. الصفحة البلاستيكية ألدوس خمسة مطبعة النشر دليل منذ مستخدماً بايج."
    ///             }
    ///         },
    ///     ]
    /// </remarks>
    /// <response code="200">Returns the all the shifts as paginated.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


    /// <summary>
    /// Get shift by id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the shifts along with the subjects.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /shifts/89967524-58c1-4a32-bd17-bfbe573034cc
    ///         {
    ///             "id": "89967524-58c1-4a32-bd17-bfbe573034cc",
    ///             "shiftStartTime": "2023-05-01T17:58:05.82Z",
    ///             "shiftEndTime": "2023-05-03T17:58:05.82Z",
    ///             "centerName": "حقيقة",
    ///             "centerId": "808e2447-bc6e-cc57-e2fb-07aa702a94e0",
    ///             "adminName": "Admin Center 2",
    ///             "adminId": "2fb8d84f-f9da-4ed8-b5d2-2f68a049b1e7",
    ///             "subjects": [
    ///             {
    ///                 "id": "f28ce390-ddc5-42ae-9a80-d91de0a6c62c",
    ///                 "name": "connect",
    ///                 "description": "هناك ظهور الخارجي مستخدماً بمعنى لينتشر المطابع وليس طبيعياَ وعاد. يقرأها ودور ظهور. ويُستخدم أو لهذه. من ولذلك وعاد مؤخراَ أو طبيعياَ. استخدام سيلهي شكل قرون عشر الإلكتروني. التي لأنها ولذلك. ألدوس النص خمسة أيضاً الزمن حقيقة الخارجي وهي."
    ///             },
    ///             {
    ///                 "id": "09be7293-32ab-43a7-aad6-5cb08ceed47d",
    ///                 "name": "back up",
    ///                 "description": "الزمن برص أخذتها إيبسوم المطابع حتى لهذه الصفحة استخدام قرون. بمعنى مجهولة مطبعة الشكلي طبيعياَ زمن الشكلي. المعيار حتى الأصلي نسخ ويُستخدم مرة ليتراسيت. الصفحة البلاستيكية ألدوس خمسة مطبعة النشر دليل منذ مستخدماً بايج."
    ///             }
    ///         }
    /// </remarks>
    /// <response code="200">Get shift by id.</response>
    /// <response code="404">Shit does not exist.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpGet("{shiftId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


    /// <summary>
    /// Create a shift endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /shifts
    ///     {
    ///         "shiftStartTime": "2023-05-01T17:58:05.820Z",
    ///         "shiftEndTime": "2023-05-03T17:58:05.820Z",
    ///         "centerId": "808e2447-bc6e-cc57-e2fb-07aa702a94e0",
    ///         "adminId": "2fb8d84f-f9da-4ed8-b5d2-2f68a049b1e7",
    ///         "subjectIds": [
    ///             "a3cec1a5-ec78-1615-1d7d-02f1aa2a9b29",
    ///             "77ce6da3-6889-8a7a-4076-06891d3e6e7d",
    ///             "5b7f6643-f5d8-c9d3-a230-1599df12aee1"
    ///         ]
    ///      }
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


    /// <summary>
    /// Update shift endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /shifts/808e2447-bc6e-cc57-e2fb-07aa702a94e0
    ///     {
    ///         "shiftStartTime": "2023-05-01T17:58:05.820Z",
    ///         "shiftEndTime": "2023-05-08T17:58:05.820Z",
    ///         "centerId": "808e2447-bc6e-cc57-e2fb-07aa702a94e0",
    ///         "adminId": "2fb8d84f-f9da-4ed8-b5d2-2f68a049b1e7",
    ///         "subjectIds": [
    ///             "a3cec1a5-ec78-1615-1d7d-02f1aa2a9b29"
    ///         ]
    ///      }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpPut("{shiftId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


    /// <summary>
    /// Remove a shift endpoint by shift id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /shifts/c29638cb-d0eb-4c9e-0518-08db45a6cc76
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [HttpDelete("{shiftId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
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
