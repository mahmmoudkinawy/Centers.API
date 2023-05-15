namespace Centers.API.Processes.Shifts;
public sealed class UpdateShiftCapacityForCenterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public int? Capacity { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CentersDbContext _context;

        public Validator(
            IHttpContextAccessor httpContextAccessor,
            CentersDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            RuleFor(r => r.Capacity)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .Must(capacity =>
                {
                    var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();
                    var centerIdFromRoute = requestRouteQuery!.Values["centerId"];
                    var centerId = Guid.Parse(centerIdFromRoute.ToString());

                    var center = _context.Centers.Find(centerId);

                    if (center is null)
                    {
                        return false;
                    }

                    return capacity.Value <= center.Capacity;
                })
                .WithMessage("Shift capacity can not exceed the center capacity.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            CentersDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var centerIdFromRoute = requestRouteQuery!.Values["centerId"];
            var shiftIdFromRoute = requestRouteQuery!.Values["shiftId"];

            var centerId = Guid.Parse(centerIdFromRoute.ToString());
            var shiftId = Guid.Parse(shiftIdFromRoute.ToString());

            var shift = await _context.Shifts
                .FirstOrDefaultAsync(s => s.Id == shiftId && s.CenterId == centerId,
                    cancellationToken: cancellationToken);

            if (shift is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the shift with the given ID does not exist or the shift does not belong to the center. Please check the ID and try again." });
            }

            shift.Capacity = request.Capacity;

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
          new List<string> { "We're sorry, but there was an error updating the shift Capacity to the database. Please try again later." });

        }
    }

}
