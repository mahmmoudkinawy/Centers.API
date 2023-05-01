namespace Centers.API.Processes.Users;
public sealed class ActivateUserAccountByAdminProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public bool IsActive { get; set; }
    }

    public sealed class Response { }

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
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var userIdFromRoute = requestRouteQuery!.Values["userId"];

            var userToUpdateId = Guid.Parse(userIdFromRoute.ToString());

            var currentUserId = _httpContextAccessor.HttpContext?.User?.GetUserById();

            if (currentUserId == userToUpdateId)
            {
                return Result<Response>.Failure(new List<string>
                {
                   "As a super admin, you cannot deactivate your account since you hold the highest level of authority."
                });
            }

            var user = await _userManager.FindByIdAsync(userToUpdateId.ToString());

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The provided ID does not exist for any user."
                });
            }

            user.IsActive = request.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Result<Response>.Failure(result.Errors.Select(e => e.Description).ToList());
            }

            return Result<Response>.Success(new Response { });
        }

    }

}
