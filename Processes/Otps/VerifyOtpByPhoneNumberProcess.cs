namespace Centers.API.Processes.Otps;
public sealed class VerifyOtpByPhoneNumberProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? PhoneNumber { get; set; }
        public string? Otp { get; set; }
    }

    public sealed class Response
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Token { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;

        public Handler(
            UserManager<UserEntity> userManager,
            ITokenService tokenService,
            IOtpService otpService)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ??
                throw new ArgumentNullException(nameof(tokenService));
            _otpService = otpService ??
                throw new ArgumentNullException(nameof(otpService));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(p => p.PhoneNumber.Equals(request.PhoneNumber), cancellationToken);

            if (user is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "We're sorry, but the phone number you provided does not appear to be registered with us."
                });
            }

            var isValidOtp = await _otpService.ValidateOtpAsync(request.PhoneNumber, request.Otp);

            if (!isValidOtp)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The OTP you entered is incorrect."
                });
            }

            var otpEntity = await _otpService.GetOtpByPhoneNumberAsync(request.PhoneNumber);

            await _otpService.RemoveOtpByPhoneNumber(otpEntity);

            return Result<Response>.Success(new Response
            {
                Name = $"{user.FirstName} {user.LastName}",
                ImageUrl = null,
                Token = await _tokenService.CreateTokenAsync(user)
            });
        }

    }

}
