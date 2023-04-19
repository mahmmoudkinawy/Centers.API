namespace Centers.API.Processes.Account;
public sealed class UserRegisterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public bool HasDisability { get; set; } = false;
        public IFormFile? DisabilityImage { get; set; }
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

            RuleFor(u => u.PhoneNumber)
                .Matches("^\\+971(\\s*|\\-)(50|51|52|55|56|2|3|4|6|7|9)\\d{7}$")
                .WithMessage("Your phone number does not appear to be valid for UAE.")
                .NotEmpty();

            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty();

            RuleFor(u => u.HasDisability)
                .NotNull();

            RuleFor(u => u.DisabilityImage)
                .Cascade(CascadeMode.Stop)
                .Must((request, file) =>
                    {
                        if (!request.HasDisability)
                        {
                            return true;
                        }

                        if (file is null)
                        {
                            return false;
                        }

                        if (file.Length > 10 * 1024 * 1024)
                        {
                            return false;
                        }

                        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(file.FileName);

                        return validExtensions.Contains(extension);
                    })
                .WithMessage("Disability Image must be an image file with a valid extension (jpg, jpeg, png, gif) and less than or equal to 10MB.")
                .When(u => u.HasDisability);
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
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IServiceScopeFactory serviceScopeFactory,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _serviceScopeFactory = serviceScopeFactory ??
                throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ??
                throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<UserEntity>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = new List<string>();
                foreach (var error in result.Errors)
                {
                    errors.Add(error.Description);
                }

                return Result<Response>.Failure(errors);
            }

            if (request.HasDisability && request.DisabilityImage is not null)
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<CentersDbContext>();
                    var photoService = scope.ServiceProvider.GetRequiredService<IPhotoService>();

                    var disabilityImageUrl = await photoService.UploadPhotoAsync(request.DisabilityImage);

                    var disability = new DisabilityEntity
                    {
                        Id = Guid.NewGuid(),
                        HasDisability = request.HasDisability,
                        ImageUrl = disabilityImageUrl,
                        OwnerId = user.Id
                    };

                    context.Disabilities.Add(disability);
                    await context.SaveChangesAsync(cancellationToken);
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
