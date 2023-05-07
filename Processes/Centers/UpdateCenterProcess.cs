namespace Centers.API.Processes.Subjects;
public sealed class UpdateCenterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Zone { get; set; }
        public string? LocationUrl { get; set; }
        public int? Capacity { get; set; }
        public bool? IsEnabled { get; set; }
    }

    public sealed class Response { }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, CenterEntity>();
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .MaximumLength(100)
                .NotEmpty()
                .NotNull();

            RuleFor(c => c.Gender)
                .NotEmpty()
                .NotNull()
                .Must(gender =>
                {
                    var genders = new[]
                    {
                        "Male",
                        "Female",
                        "Both"
                    };

                    return genders.Contains(gender, StringComparer.OrdinalIgnoreCase);
                })
                .WithMessage("Gender should be either Male, Female, or Both.");

            RuleFor(c => c.Capacity)
                .GreaterThan(0)
                .NotEmpty()
                .NotNull();

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

            RuleFor(c => c.LocationUrl)
                .NotEmpty()
                .NotNull()
                .Matches(@"^(https?://)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*/?$")
                .WithMessage("LocationUrl should be a valid URL.");

            RuleFor(c => c.IsEnabled)
                .NotEmpty()
                .NotNull();

        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            CentersDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var centerIdFromRoute = requestRouteQuery!.Values["centerId"];

            var centerId = Guid.Parse(centerIdFromRoute.ToString());

            var center = await _context.Centers.FindAsync(
                new object?[] { centerId },
                cancellationToken: cancellationToken);

            if (center is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the center with the given ID does not exist. Please check the ID and try again." });
            }

            _mapper.Map(request, center);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the center to the database. Please try again later." });
        }
    }

}
