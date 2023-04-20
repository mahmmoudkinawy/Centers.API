namespace Centers.API.Processes.Otps;
public sealed class SendOtpToConfirmPhoneNumberProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? PhoneNumber { get; set; }
    }

    public sealed class Response
    {
        public string Message { get; set; }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            // UAE Validation for phone number.
            //RuleFor(r => r.PhoneNumber)
            //    .NotEmpty()
            //    .Must(p => p.StartsWith("+971"))
            //    .WithMessage("Invalid phone number. The phone number should start with +971.")
            //    .Matches(@"^\+9715[0-9]\d{7}$")
            //    .WithMessage("Invalid phone number. The phone number should start with +971, followed by a valid phone number.");

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

        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IOtpService _otpService;
        private readonly ISmsService _smsService;

        public Handler(
            UserManager<UserEntity> userManager,
            IOtpService otpService,
            ISmsService smsService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _otpService = otpService ??
                throw new ArgumentNullException(nameof(otpService));
            _smsService = smsService ??
                throw new ArgumentNullException(nameof(smsService));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            if (!await _userManager.Users
                .AnyAsync(p => p.PhoneNumber.Equals(request.PhoneNumber), cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The phone number you have selected does not exist in the database."
                });
            }

            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(request.PhoneNumber),
                    cancellationToken);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "We're sorry, but the phone number you provided does not appear to be registered with us."
                });
            }

            var otpCode = await _otpService.GenerateOtpAsync(6);

            if (otpCode is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "We're sorry, but the OTP you entered is either incorrect or has expired. Please try again with a new OTP."
                });
            }

            var currentUserOtps = await _otpService.GetOtpsByPhoneNumberAsync(request.PhoneNumber);

            await _otpService.RemoveOtps(currentUserOtps);

            var sendSms = await _smsService.SendSmsAsync(
                                  $"{request.PhoneNumber}",
                                  $"Thank you for registering! To verify your account, please enter the following OTP code: {otpCode}.");

            if (!string.IsNullOrWhiteSpace(sendSms.ErrorMessage))
            {
                return Result<Response>.Failure(new List<string>
                {
                    sendSms.ErrorMessage
                });
            }

            await _otpService.StoreOtp(request.PhoneNumber, otpCode);

            return Result<Response>.Success(new Response
            {
                Message = "Great news! We have sent an OTP to your phone number."
            });
        }

    }
}
