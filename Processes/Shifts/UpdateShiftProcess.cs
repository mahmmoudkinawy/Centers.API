namespace Centers.API.Processes.Shifts;
public sealed class UpdateShiftProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? ShiftStartTime { get; set; }
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
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            RuleFor(r => r.ShiftStartTime)
                .NotNull();

            RuleFor(r => r.Capacity)
                .NotNull()
                .GreaterThan(0)
                .Must((request, capacity) =>
                {
                    var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();
                    var centerIdFromRoute = requestRouteQuery!.Values["centerId"];
                    var centerId = Guid.Parse(centerIdFromRoute.ToString());

                    var center = _context.Centers.Find(centerId);

                    if (center == null)
                    {
                        return false;
                    }

                    return center.Capacity >= request.Capacity;
                })
                .WithMessage("The capacity of the reservation must not exceed the original capacity of the center it is reserved for.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            CentersDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(mapper));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var shiftIdFromRoute = requestRouteQuery!.Values["shiftId"];

            var shiftId = Guid.Parse(shiftIdFromRoute.ToString());

            var shift = await _context.Shifts
                .FirstOrDefaultAsync(s => s.Id == shiftId, cancellationToken: cancellationToken);

            if (shift is null)
            {
                return Result<Response>.Failure(
                    new List<string> { "We're sorry, but the shift with the given ID does not exist. Please check the ID and try again." });
            }

            //foreach (var subjectId in request.SubjectIds)
            //{
            //    var existingShiftSubject = shift.ShiftSubjects.FirstOrDefault(s => s.SubjectId == subjectId);
            //    if (existingShiftSubject is not null)
            //    {
            //        _context.ShiftSubjects.Update(existingShiftSubject);
            //    }
            //    else
            //    {
            //        var shiftSubject = new ShiftSubjectEntity
            //        {
            //            Id = Guid.NewGuid(),
            //            ShiftId = shift.Id,
            //            SubjectId = subjectId
            //        };

            //        _context.ShiftSubjects.Add(shiftSubject);
            //    }
            //}

            //_mapper.Map(request, shift);

            //if (await _context.Shifts.AnyAsync(s =>
            //    s.Id != shiftId &&
            //    //s.AdminId == request.AdminId &&
            //    s.ShiftStartTime <= request.ShiftEndTime &&
            //    s.ShiftEndTime >= request.ShiftStartTime, cancellationToken))
            //{
            //    return Result<Response>.Failure(new List<string>
            //    {
            //        "A shift already exists at the specified time for the selected center and admin. Please choose a different time or contact the admin for assistance."
            //    });
            //}

            //if (await _context.SaveChangesAsync(cancellationToken) > 0)
            //{
            //    return Result<Response>.Success(new Response { });
            //}

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error updating the shift to the database. Please try again later." });
        }


    }
}
