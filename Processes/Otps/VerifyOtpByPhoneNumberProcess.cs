namespace Centers.API.Processes.Otps;
public sealed class VerifyOtpByPhoneNumberProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? PhoneNumber { get; set; }
        public string? OtpCode { get; set; }
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

            RuleFor(r => r.OtpCode)
                 .NotNull()
                 .NotEmpty();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IOtpService _otpService;

        public Handler(
            UserManager<UserEntity> userManager,
            IOtpService otpService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _otpService = otpService ??
                throw new ArgumentNullException(nameof(otpService));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
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

            var isValidOtp = await _otpService.ValidateOtpAsync(request.PhoneNumber, request.OtpCode);

            if (!isValidOtp)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "We're sorry, but the OTP you entered is either incorrect or has expired. Please try again with a new OTP."
                });
            }

            var otpEntity = await _otpService.GetOtpByPhoneNumberAsync(request.PhoneNumber);

            await _otpService.RemoveOtp(otpEntity);

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Result<Response>.Success(new Response
            {
                Message = "Great news! Your phone number has been verified, and you can now log in with confidence."
            });
        }

    }

}
