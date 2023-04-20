namespace Centers.API.Processes.Account;
public sealed class UserLoginProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public sealed class Response
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Token { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;

        public Handler(
            UserManager<UserEntity> userManager,
            ITokenService tokenService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ??
                throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Invalid email address or password. Please check your credentials and try again."
                });
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Invalid email address or password. Please check your credentials and try again."
                });
            }

            if (!await _userManager.IsPhoneNumberConfirmedAsync(user))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Your phone number has not yet been verified. To continue, please confirm your phone number first."
                });
            }

            return Result<Response>.Success(new Response
            {
                Name = $"{user.FirstName} {user.LastName}",
                ImageUrl = null,
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }

    }

}
