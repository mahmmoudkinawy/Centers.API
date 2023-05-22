namespace Centers.API.Processes.Users;
public sealed class CreateUserProcess
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
        public string? Password { get; set; }
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

            RuleFor(u => u.Password)
                .NotEmpty();

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
            CreateMap<Request, UserEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(u => Guid.NewGuid()))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(r => r.Email));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<UserEntity>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Result<Response>.Failure(result.Errors.Select(e => e.Description).ToList());
            }

            var roles = new List<string>()
            {
                Constants.Roles.Reviewer,
                Constants.Roles.Teacher,
                Constants.Roles.CenterAdmin,
                Constants.Roles.SuperAdmin
            };

            var userRole = roles.FirstOrDefault(r =>
                r.Contains(request.Role, StringComparison.InvariantCultureIgnoreCase));

            if (userRole is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The role that you are trying to add this user to does not exist."
                });
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, userRole);

            if (!addToRoleResult.Succeeded)
            {
                return Result<Response>.Failure(addToRoleResult.Errors.Select(e => e.Description).ToList());
            }

            if (request.IsPhoneNumberConfirmed!.Value)
            {
                user.PhoneNumberConfirmed = true;
                await _userManager.UpdateAsync(user);

                return Result<Response>.Success(new Response
                {
                    Message = "The user has been created successfully. With the credentials you have set, the user can now log in."
                });
            }

            return Result<Response>.Success(new Response
            {
                Message = "The user has been created successfully. The next step is for the user to confirm their phone number."
            });
        }

    }

}
