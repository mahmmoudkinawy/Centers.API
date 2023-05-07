namespace Centers.API.Processes.Shifts;
public sealed class CreateShiftProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? ShiftStartTime { get; set; }
        public DateTime? ShiftEndTime { get; set; }
        public int? Capacity { get; set; }
        public Guid CenterId { get; set; }
        public Guid AdminId { get; set; }
        public ICollection<Guid> SubjectIds { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly CentersDbContext _context;

        public Validator(CentersDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            RuleFor(r => r.ShiftStartTime)
                .NotNull();

            RuleFor(r => r.ShiftEndTime)
                .NotNull()
                .GreaterThan(r => r.ShiftStartTime);

            RuleFor(r => r.Capacity)
                .NotNull()
                .GreaterThan(0)
                .Must((request, capacity) =>
                {
                    var center = _context.Centers.Find(request.CenterId);

                    if (center == null)
                    {
                        return false;
                    }

                    return center.Capacity >= request.Capacity;
                })
                .WithMessage("The capacity of the reservation must not exceed the original capacity of the center it is reserved for.");

            RuleFor(r => r.CenterId)
                .NotEqual(r => r.AdminId)
                .WithMessage("CenterId and AdminId can not be the same.");

            RuleFor(r => r.CenterId)
                .NotEmpty()
                .Must((centerId) => _context.Centers.Any(u => u.Id == centerId))
                .WithMessage("No center exists in the database with the provided CenterId.");

            RuleFor(r => r.AdminId)
                .Must((adminId) =>
                {
                    // May be will make it that one admin only for one center
                    var user = _context.Users.Find(adminId);
                    if (user == null)
                    {
                        return false;
                    }

                    var adminCenterRoleId = _context.Roles
                        .Where(r => r.Name == Constants.Roles.CenterAdmin)
                        .Select(r => r.Id)
                        .SingleOrDefault();

                    return _context.UserRoles
                        .Any(ur => ur.UserId == adminId && ur.RoleId == adminCenterRoleId);
                })
                .WithMessage("User is not assigned to the Admin Center role.");

            // Maybe we can make this as Distinct in the future.
            RuleForEach(r => r.SubjectIds)
                .NotEmpty()
                .Must((subjectId) => _context.Subjects.Any(s => s.Id == subjectId))
                .WithMessage("The provided SubjectId does not match any record in the database.");
        }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, ShiftEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(s => Guid.NewGuid()))
                .ForMember(dest => dest.ShiftSubjects,
                    opt => opt.MapFrom(s => s.SubjectIds.Select(sId => new ShiftSubjectEntity
                    {
                        Id = Guid.NewGuid(),
                        SubjectId = sId
                    }).ToList()));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;

        public Handler(
            CentersDbContext context,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }


        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var shift = _mapper.Map<ShiftEntity>(request);

            if (await _context.Shifts.AnyAsync(s =>
                s.AdminId == shift.AdminId &&
                s.ShiftStartTime <= shift.ShiftEndTime &&
                s.ShiftEndTime >= shift.ShiftStartTime, cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "A shift already exists at the specified time for the selected center and admin. Please choose a different time or contact the admin for assistance."
                });
            }

            _context.Shifts.Add(shift);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Unable to create shift. Please try again later."
            });

        }

    }

}
