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

    [Authorize(Policy = Constants.Policies.MustBeTeacher)]
    [HttpPost]
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

}
