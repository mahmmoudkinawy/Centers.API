namespace Centers.API.Processes.Users;
public sealed class RemoveUserProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid? UserId { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty()
                .NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            if (currentUserId == request.UserId)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "As an admin, you cannot remove yourself."
                });
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "A user with the provided ID does not exist."
                });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return Result<Response>.Failure(result.Errors.Select(e => e.Description).ToList());
            }

            return Result<Response>.Success(new Response
            {
                Message = "The user with the provided ID has been successfully removed."
            });
        }

    }
}
