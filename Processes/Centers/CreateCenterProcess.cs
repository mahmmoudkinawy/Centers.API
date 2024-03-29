﻿namespace Centers.API.Processes.Subjects;
public sealed class CreateCenterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Zone { get; set; }
        public string? LocationUrl { get; set; }
        public int? Capacity { get; set; }
        public bool? IsEnabled { get; set; }
        public Guid? OwnerId { get; set; }

        public DateTime? ShiftStartTime { get; set; } = DateTime.UtcNow.Date.AddHours(10); // Starting time of the first shift
        public TimeSpan? ShiftDuration { get; set; } = TimeSpan.FromHours(2); // Duration of each shift
    }

    public sealed class Response { }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, CenterEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly CentersDbContext _context;

        public Validator(CentersDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

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

            RuleFor(c => c.OwnerId)
                .Must(ownerId =>
                {
                    var isCenterAdmin = _context.UserRoles
                        .Any(ur => ur.UserId == ownerId && _context.Roles
                            .Any(r => r.Id == ur.RoleId && r.Name == Constants.Roles.CenterAdmin));

                    var isAdminInOtherCenter = _context.Centers
                        .Any(c => c.OwnerId == ownerId);

                    return isCenterAdmin && !isAdminInOtherCenter;
                })
                .WithMessage("The OwnerId must belong to the Center Admin role, and a user can only be associated as an admin with one center at a time. The provided ownerId is either not associated with the Center Admin role or is already associated with another center as an admin.");
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
            var center = _mapper.Map<CenterEntity>(request);

            var shiftStart = request.ShiftStartTime ?? DateTime.UtcNow;
            var shiftDuration = request.ShiftDuration ?? TimeSpan.FromHours(2);

            for (var i = 0; i < 4; i++)
            {
                var shift = new ShiftEntity
                {
                    Id = Guid.NewGuid(),
                    ShiftStartTime = shiftStart,
                    ShiftEndTime = shiftStart.Add(shiftDuration),
                    Center = center,
                    Capacity = 20,  // Shift Capacity not center Capacity.
                    IsEnabled = false
                };

                center.Shifts.Add(shift);

                shiftStart = shift.ShiftStartTime.Value.AddHours(2);
            }

            _context.Centers.Add(center);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the center to the database. Please try again later." });
        }
    }

}
