namespace Centers.API.Processes.Users;
public sealed class UpdateUserProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? Zone { get; set; }
        public string? PhoneNumber { get; set; }
        public string? NationalId { get; set; }
        public string? Email { get; set; }
        public bool? IsPhoneNumberConfirmed { get; set; } = false;
        public string? Role { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(u => u.FirstName)
                .MinimumLength(3)
                .MaximumLength(50)
                .NotEmpty();

            RuleFor(u => u.LastName)
                .MinimumLength(3)
                .MaximumLength(50)
                .NotEmpty();

            RuleFor(u => u.Gender)
                .NotEmpty();

            RuleFor(c => c.Zone)
              .NotEmpty()
              .NotNull()
              .Must(zone =>
              {
                  var zones = new[]
                  {
                        "Abu Dhabi",
                        "Ajman",
                        "Dubai",
                        "Fujairah",
                        "Ras al-Khaimah",
                        "Sharjah",
                        "Umm al-Quwain"
                  };

                  return zones.Contains(zone, StringComparer.OrdinalIgnoreCase);
              });


            //RuleFor(u => u.PhoneNumber)
            //    .Matches("^\\+971\\s*(50|51|52|55|56|2|3|4|6|7|9)\\d{7}$")
            //    .WithMessage("Your phone number does not appear to be valid for UAE.")
            //    .NotEmpty();

            // UAE phone number validation.
            //RuleFor(r => r.PhoneNumber)
            //    .NotEmpty()
            //    .Must(p => p.StartsWith("+971"))
            //    .WithMessage("Invalid phone number. The phone number should start with +971.")
            //    .Matches(@"^\+9715[0-9]\d{7}$")
            //    .WithMessage("Your phone number does not appear to be valid for UAE.");

            // Egyptian Phone number validation will be removed.
            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .NotNull()
                .WithMessage("Phone number is required.")
                .Must(p => p != null && p.StartsWith("+2"))
                .WithMessage("Invalid phone number. The phone number should start with +2.")
                .WithMessage("Phone number is required.")
                .Matches(@"^\+201[0125][0-9]{8}$")
                .WithMessage("Your phone number does not appear to be valid for Egypt.");

            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty();

            //RuleFor(u => u.NationalId)
            //    .Matches("^784-(19|20)\\d{2}-\\d{7}-\\d{1}$")
            //    .WithMessage("Your National Id does not appear to be valid for UAE.")
            //    .NotEmpty();

            RuleFor(u => u.Role)
                .NotEmpty()
                .NotNull()
                .Must(role =>
                {
                    var roles = new[]
                    {
                        Constants.Roles.CenterAdmin,
                        Constants.Roles.Reviewer,
                        Constants.Roles.Student,
                        Constants.Roles.Teacher
                    };

                    return roles.Contains(role);
                });
        }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, UserEntity>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var userIdFromRoute = requestRouteQuery!.Values["userId"];

            var userToUpdateId = Guid.Parse(userIdFromRoute.ToString());

            var currentUserId = _httpContextAccessor.HttpContext?.User?.GetUserById();

            if (userToUpdateId == currentUserId)
            {
                return Result<Response>.Failure(new List<string> { "As the admin, you are not authorized to update your personal data from here." });
            }

            if (await _userManager.Users
               .AnyAsync(p => p.PhoneNumber.Equals(request.PhoneNumber), cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The phone number you've chosen is currently associated with another account and has already been verified."
                });
            }

            var user = await _userManager.FindByIdAsync(userToUpdateId.ToString());

            if (user is null)
            {
                return Result<Response>.Failure(new List<string> { "We're sorry, but we could not find a user with the provided id." });
            }

            _mapper.Map(request, user);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Result<Response>.Failure(updateResult.Errors.Select(e => e.Description).ToList());
            }

            var roles = new List<string>()
            {
                // Admin can not add admin till now.
                Constants.Roles.Student,
                Constants.Roles.Reviewer,
                Constants.Roles.Teacher,
                Constants.Roles.CenterAdmin
            };

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var userRoleToUpdate = roles.FirstOrDefault(r =>
                r.Contains(request.Role, StringComparison.InvariantCultureIgnoreCase));

            if (userRoleToUpdate is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The role that you are trying to add this user to does not exist."
                });
            }

            var updateToRoleResult = await _userManager.AddToRoleAsync(user, userRoleToUpdate);

            if (!updateToRoleResult.Succeeded)
            {
                return Result<Response>.Failure(updateToRoleResult.Errors.Select(e => e.Description).ToList());
            }

            if (request.IsPhoneNumberConfirmed!.Value)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);

                return Result<Response>.Success(new Response
                {
                    Message = "The user has been updated successfully. With the credentials you have set, the user can now log in."
                });
            }

            return Result<Response>.Success(new Response
            {
                Message = "The user has been updated successfully. The next step is for the user to confirm their phone number."
            });
        }

    }
}
