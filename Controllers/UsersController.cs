namespace Centers.API.Controllers;

[Route("api/v{version:apiVersion}/users")]
[ApiController]
[ApiVersion("1.0")]
public sealed class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }

    /// <summary>
    /// current user endpoint to get the current user profile.
    /// </summary>
    /// <returns>Returns the user data from the eb as the model will describe.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /current-user-profile
    /// </remarks>
    /// <response code="200">Returns the user data from the eb as the model will describe.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize]
    [HttpGet("current-user-profile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUserProfile(
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetCurrentUserProfileProcess.Request { },
            cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Get all users endpoint to list the users.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the users</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /users
    ///     {    
    ///         "id": "634df3ad-cb41-4ba2-90e0-0f0390c43932",
    ///         "fullName": "Mahmoud Kinawy",
    ///         "gender": "Male",
    ///         "nationalId": "784-1997-1234567-8",
    ///         "phoneNumber": "+201208534244",
    ///         "email": "mahmoud.kinawy@test.com"
    ///     },
    ///     {    
    ///         "id": "cc75d6d6-a463-40b1-981a-405e5b759716",
    ///         "fullName": "Isseo Yasso",
    ///         "gender": "Female",
    ///         "nationalId": "784-1997-1231267-8",
    ///         "phoneNumber": "+201208534263",
    ///         "email": "Isseo.uasso@test.com"
    ///     }
    /// </remarks>
    /// <response code="200">Returns the all the users.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers(
            [FromQuery] UserParams userParams,
            CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new GetUsersProcess.Request
            {
                Keyword = userParams.Keyword,
                PageNumber = userParams.PageNumber,
                PageSize = userParams.PageSize
            },
            cancellationToken);

        Response.AddPaginationHeader(
            response.CurrentPage,
            response.PageSize,
            response.TotalPages,
            response.TotalCount);

        return Ok(response);
    }

    /// <summary>
    /// Create a user with some role.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns what is the next step that user will do to use the account.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /users
    ///     {
    ///         "firstName": "Sameh",
    ///         "lastName": "Yossry",
    ///         "gender": "Male",
    ///         "phoneNumber": "+201208534246",
    ///         "nationalId": "789-1234-5678901-2",
    ///         "email": "sameh.yossry@test.com",
    ///         "password": "Pa$$w0rd",
    ///         "isPhoneNumberConfirmed": true,
    ///         "role": "Teacher"
    ///     }
    /// </remarks>
    /// <response code="200">Returns what is the next step that user will do to use the account..</response>
    /// <response code="401">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return Unauthorized(response.Errors);
        }

        return Ok(response.Value);
    }

    /// <summary>
    /// Updated a user with the role.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns if the whole operation succeed or not.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /users/298b38ca-2325-b88a-43ee-0371425a26ce
    ///     {
    ///         "firstName": "Sameh updated",
    ///         "lastName": "Yossry updated",
    ///         "gender": "Male",
    ///         "phoneNumber": "+201208534246",
    ///         "nationalId": "789-1234-5678901-2",
    ///         "email": "sameh.yossry@test.com",
    ///         "password": "Pa$$w0rd",
    ///         "isPhoneNumberConfirmed": true,
    ///         "role": "Student"
    ///     }
    /// </remarks>
    /// <response code="200">Returns if the whole operation succeed or not.</response>
    /// <response code="401">Validation errors.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateUser(
        [FromBody] UpdateUserProcess.Request request,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            request,
            cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return Ok(response.Value);
    }

    /// <summary>
    /// Remove some user with the given ID.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns the all the users</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /users/cc75d6d6-a463-40b1-981a-405e5b759716
    /// </remarks>
    /// <response code="200">Returns the all the users.</response>
    /// <response code="401">User does not exist.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RemoveUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(
            new RemoveUserProcess.Request
            {
                UserId = userId
            }, cancellationToken);

        if (!response.IsSuccess)
        {
            return BadRequest(response.Errors);
        }

        return NoContent();
    }


    /// <summary>
    /// Activate some User active or not by admin.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="userUpdateStatus"></param>
    /// <returns>Returns no content</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT users/be0b6bca-ca43-426e-9024-ec1a52e3235a
    ///     {
    ///         "isActive":false
    ///     }
    /// </remarks>
    /// <response code="204">Returns no content if the operation succeeded.</response>
    /// <response code="400">If the update got some error.</response>
    /// <response code="404">User with the given id does not exist.</response>
    [Authorize(Policy = Constants.Policies.MustBeSuperAdmin)]
    [HttpPut("{userId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateUserBySuperAdmin(
        [FromBody] ActivateUserAccountByAdminProcess.Request request,
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
