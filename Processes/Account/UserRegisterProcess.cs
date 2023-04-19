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
                //.Matches("^(5[0-9]{1}-[0-9]{3}-[0-9]{4})$")
                //.WithMessage("Your phone number does not appear to be valid for Dubai. Please enter a 10-digit phone number starting with 05.")
                .NotEmpty();

            RuleFor(u => u.Email)
                .EmailAddress()
                .NotEmpty();

            RuleFor(u => u.Password)
                .NotEmpty();

            RuleFor(u => u.HasDisability)
                .NotEmpty();

            RuleFor(u => u.DisabilityImage)
                .NotEmpty()
                .When(u => u.HasDisability)
                .WithMessage("Disability Image is required when HasDisability is true.")
                .Must(file =>
                {
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
                .WithMessage("Disability Image must be an image file with a valid extension (jpg, jpeg, png, gif) and less than or equal to 10MB.");
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
        private readonly CentersDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            CentersDbContext context,
            ITokenService tokenService,
            IPhotoService photoService,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _context = context ??
                throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ??
                throw new ArgumentNullException(nameof(tokenService));
            _photoService = photoService ??
                throw new ArgumentNullException(nameof(photoService));
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
                var disabilityImageUrl = await _photoService.UploadPhotoAsync(request.DisabilityImage);

                var disability = new DisabilityEntity
                {
                    Id = Guid.NewGuid(),
                    HasDisability = request.HasDisability,
                    ImageUrl = disabilityImageUrl,
                    OwnerId = user.Id
                };

                _context.Disabilities.Add(disability);
                await _context.SaveChangesAsync(cancellationToken);
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
