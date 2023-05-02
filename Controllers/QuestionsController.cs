namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/questions")]
[ApiController]
[ApiVersion("1.0")]
public sealed class QuestionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public QuestionsController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Create a question 'MCQ-True/False-Free Text' endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /questions
    ///     {
    ///         "text": "string",
    ///         "type": 2,
    ///         "choices": [
    ///             {
    ///                 "text": "string",
    ///                 "isCorrect": true
    ///             },
    ///             {
    ///                 "text": "string 1",
    ///                 "isCorrect": false
    ///             },
    ///         ],
    ///         "answerText": "string"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateQuestion(
        [FromBody] CreateQuestionProcess.Request request,
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
    /// Create a question 'MCQ-True/False-Free Text' endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /questions/ce55c93a-764d-4b54-8503-de419aa641d3
    ///     {
    ///         "text": "string test",
    ///         "type": 2,
    ///         "choices": [
    ///             {
    ///                 "text": "string 2",
    ///                 "isCorrect": false
    ///             },
    ///             {
    ///                 "text": "string UPDATED",
    ///                 "isCorrect": true
    ///             },
    ///             {
    ///                 "text": "string UPDATED 2",
    ///                 "isCorrect": false
    ///             },
    ///         ],
    ///         "answerText": "string"
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpPut("{questionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateQuestion(
      [FromBody] UpdateQuestionProcess.Request request,
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
    /// Remove a question endpoint by question id.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /questions/c29638cb-d0eb-4c9e-0518-08db45a6cc76
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpDelete("{questionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteQuestion(
        [FromRoute] Guid questionId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveQuestionProcess.Request
            {
                QuestionId = questionId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }

}
