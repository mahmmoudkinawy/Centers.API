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
    /// Get paginated questions for the current logged in user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the paginated questions for the current logged in user.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /questions/current-user-questions-with-answers
    ///     [
    ///         {
    ///             "id": "f969f33a-651d-46fb-9362-aa58daa14c98",
    ///             "questionText": "string",
    ///             "type": "FreeText",
    ///             "choices": [],
    ///             "answerText": "string"
    ///          },
    ///          {
    ///             "id": "78edd456-29f8-4a74-86f1-c028080293e1",
    ///             "questionText": "string",
    ///             "type": "TrueFalse",
    ///             "choices": [
    ///                 {
    ///                     "id": "cb0e02e5-9e66-4e6e-b5d4-af2f01a0cc6e",
    ///                     "text": "string",
    ///                     "isCorrect": true
    ///                 },
    ///                 {
    ///                     "id": "f91b357f-b357-4e25-8d6e-d00d79d2ac03",
    ///                     "text": "string 1",
    ///                     "isCorrect": false
    ///                 }
    ///              ],
    ///             "answerText": null
    ///         }
    ///     ]
    /// </remarks>
    /// <response code="200">Returns the paginated questions for the current logged in user.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpGet("current-user-questions-with-answers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCurrentUserQuestionsWithAnswers(
        [FromQuery] PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetQuestionsWithAnswersProcess.Request
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
    /// Get question with answer by id for the current logged in user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the question with answer by id for the current logged in user.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /questions/f969f33a-651d-46fb-9362-aa58daa14c98/current-user-question-with-answers
    ///     [
    ///         {
    ///             "id": "f969f33a-651d-46fb-9362-aa58daa14c98",
    ///             "questionText": "string",
    ///             "type": "FreeText",
    ///             "choices": [],
    ///             "answerText": "string"
    ///          }
    ///     ]
    /// </remarks>
    /// <response code="200">Get question with answers by id for the current user.</response>
    /// <response code="404">Question does not exist.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpGet("{questionId}/current-user-question-with-answers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCurrentUserQuestionsByIdWithAnswers(
        [FromRoute] Guid questionId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetQuestionByIdWithAnswersProcess.Request
            {
                QuestionId = questionId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return NotFound(response.Errors);
        }

        return Ok(response.Value);
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
    ///    POST /questions
    ///
    ///    Description: Create a new question.
    ///
    ///    Sample Request:
    ///
    ///    POST /questions
    ///    Content-Type: multipart/form-data; boundary=boundary
    ///
    ///   --boundary
    ///    Content-Disposition: form-data; name="text"
    ///    string
    ///
    ///   --boundary
    ///   Content-Disposition: form-data; name="type"
    ///   2
    ///   
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[0][text]"
    ///    
    ///   string
    ///   
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[0][isCorrect]"
    ///
    ///   true
    ///  
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[1][text]"
    ///  
    ///   string 1
    /// 
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[1][isCorrect]"
    /// 
    ///   false
    ///
    ///   --boundary
    ///   Content-Disposition: form-data; name="answerText"
    ///   
    /// string
    /// 
    /// --boundary--
    /// 
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
        [FromForm] CreateQuestionProcess.Request request,
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
    /// Exam questions selection by subject 'MCQ/FreeText'.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///    POST /questions/exam-questions-selection-by-subject/F1475DDB-835D-4F8D-843E-0B0093038F75
    ///    {
    ///         "from": "2023-05-20T08:05:54.641Z",
    ///         "to": "2023-05-27T08:05:54.641Z",
    ///         "types": [
    ///             "MCQ",
    ///             "FreeText"
    ///         ],
    ///         "questionsCount": 3
    ///    }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeReviewer)]
    [HttpPost("exam-questions-selection-by-subject/{subjectId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SubjectExamQuestionSelection(
        [FromBody] SubjectExamQuestionSelectionProcess.Request request,
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
    /// Endpoint for uploading a CSV file of questions.
    /// </summary>
    /// <param name="file">The CSV file containing questions.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns no content.</returns>
    /// <remarks>
    ///     
    ///     Sample request:
    ///
    ///         POST /questions/upload-csv
    ///         Content-Type: multipart/form-data
    ///
    ///         --boundary
    ///         Content-Disposition: form-data; name="file"; filename="questions.csv"
    ///         Content-Type: text/csv
    ///
    /// </remarks>
    /// <response code="204">Returns no content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpPost("upload-csv")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadCsvQuestionsFile(
       [FromForm] UploadQuestionsByFileProcess.Request request,
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
    /// Create a question 'MCQ - True/False - Free Text' endpoint.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///    POST /questions
    ///
    ///    Description: Create a new question.
    ///
    ///    Sample Request:
    ///
    ///    POST /questions
    ///    Content-Type: multipart/form-data; boundary=boundary
    ///
    ///   --boundary
    ///    Content-Disposition: form-data; name="text"
    ///    string
    ///
    ///   --boundary
    ///   Content-Disposition: form-data; name="type"
    ///   2
    ///   
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[0][text]"
    ///    
    ///   string updated
    ///   
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[0][isCorrect]"
    ///
    ///   true
    ///  
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[1][text]"
    ///  
    ///   string updated
    /// 
    ///   --boundary
    ///   Content-Disposition: form-data; name="choices[1][isCorrect]"
    /// 
    ///   false
    ///
    ///   --boundary
    ///   Content-Disposition: form-data; name="answerText"
    ///   
    /// string
    /// 
    /// --boundary--
    /// 
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
      [FromForm] UpdateQuestionProcess.Request request,
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
    /// Updates a question status as approved or not.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns not content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /questions/ce55c93a-764d-4b54-8503-de419aa641d3/set-question-status
    ///     {
    ///         "isApproved": true
    ///     }
    /// </remarks>
    /// <response code="204">Returns not content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    /// <response code="403">You are not authorized to perform that.</response>
    [Authorize(Policy = Constants.Policies.MustBeReviewer)]
    [HttpPut("{questionId:guid}/set-question-status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateQuestionStatus(
      [FromBody] UpdateQuestionStatusProcess.Request request,
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
