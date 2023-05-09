namespace Centers.API.Processes.Account;
public sealed class UserRegisterProcess
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

        public bool HasDisability { get; set; } = false;
        public IFormFile? DisabilityImage { get; set; }
    }

    public sealed class Response
    {
        public string PhoneNumber { get; set; }
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

            RuleFor(u => u.NationalId)
                .Matches("^784-(19|20)\\d{2}-\\d{7}-\\d{1}$")
                .WithMessage("Your National Id does not appear to be valid for UAE.")
                .NotEmpty();

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
        private readonly IMapper _mapper;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;

        public Handler(
            UserManager<UserEntity> userManager,
            IServiceScopeFactory serviceScopeFactory,
            IMapper mapper,
            IOtpService otpService,
            ISmsService smsService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _serviceScopeFactory = serviceScopeFactory ??
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _otpService = otpService ??
                throw new ArgumentNullException(nameof(otpService));
            _smsService = smsService ??
                throw new ArgumentNullException(nameof(smsService));
        }

        public async Task<Result<Response>> Handle(
            Request request, CancellationToken cancellationToken)
        {
            if (await _userManager.Users
                .AnyAsync(p => p.PhoneNumber.Equals(request.PhoneNumber), cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The phone number you've chosen is currently associated with another account and has already been verified."
                });
            }

            var user = _mapper.Map<UserEntity>(request);

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return Result<Response>.Failure(result.Errors.Select(e => e.Description).ToList());
            }

            if (request.HasDisability && request.DisabilityImage is not null)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
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
                    };
                }, cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            await _userManager.AddToRoleAsync(user, Constants.Roles.Student);

            if (!await _userManager.IsPhoneNumberConfirmedAsync(user))
            {
                var otp = await _otpService.GenerateOtpAsync(6);

                await _otpService.StoreOtp(request.PhoneNumber, otp);

                var sendSms = await _smsService
                    .SendSmsAsync(
                        $"{request.PhoneNumber}",
                        $"Thank you for registering! To verify your account, please enter the following OTP code: {otp}.");

                if (!string.IsNullOrWhiteSpace(sendSms.ErrorMessage))
                {
                    return Result<Response>.Failure(new List<string>
                    {
                        sendSms.ErrorMessage
                    });
                }

                return Result<Response>.Success(new Response
                {
                    PhoneNumber = request.PhoneNumber,
                    Message = "Please enter the OTP code sent to your phone to complete registration."
                });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Something went wrong."
            });
        }

    }

}
