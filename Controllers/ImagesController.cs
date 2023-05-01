namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/images")]
[ApiController]
[ApiVersion("1.0")]
public sealed class ImagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ImagesController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// Upload Image endpoint to upload an image for the logged in user.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns No Content.</returns>
    /// /// <remarks>
    /// Sample request:
    ///
    ///     POST /upload-image
    ///     Content-Type: multipart/form-data
    ///     
    ///     --boundary
    ///     Content-Disposition: form-data; name="image"; filename="some-image.jpg"
    ///     Content-Type: image/jpeg
    ///
    /// </remarks>
    /// <response code="204">No Content.</response>
    /// <response code="400">Validation errors.</response>
    /// <response code="401">Unauthorized - User does not exist.</response>
    [Authorize]
    [HttpPost("upload-user-profile-image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImageForUser(
        [FromForm] UserImageUploadForProfileProcess.Request request,
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
