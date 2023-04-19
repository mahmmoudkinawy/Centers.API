namespace Centers.API.Controllers;

[Route("api/account")]
[ApiController]
[AllowAnonymous]
public sealed class AccountController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountController(IMediator mediator)
    {
        _mediator = mediator ??
            throw new ArgumentNullException(nameof(mediator));
    }
}
