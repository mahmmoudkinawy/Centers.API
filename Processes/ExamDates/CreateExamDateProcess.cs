namespace Centers.API.Processes.ExamDates;
public sealed class CreateExamDateProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? Date { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public IEnumerable<Guid>? SubjectIds { get; set; }
        public IEnumerable<Guid>? CenterIds { get; set; }
    }

    public sealed class Response { }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, ExamDateEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.ExamDateSubjects, opt => opt.MapFrom(src => CreateExamDateSubjects(src.SubjectIds, src.CenterIds)));
        }

        private static IEnumerable<ExamDateSubject> CreateExamDateSubjects(
            IEnumerable<Guid> subjectIds,
            IEnumerable<Guid> centerIds)
        {
            foreach (var subjectId in subjectIds)
            {
                foreach (var centerId in centerIds)
                {
                    var examDateSubject = new ExamDateSubject
                    {
                        SubjectId = subjectId,
                        CenterId = centerId
                    };

                    yield return examDateSubject;
                }
            }
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        private readonly CentersDbContext _context;

        public Validator(CentersDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));

            // Is it okay? to choose the same subject id? in the same exam data twice? or something like that?
            // Same for center id.
            RuleFor(e => e.SubjectIds)
                .NotEmpty()
                .NotNull()
                .Must(req =>
                {
                    var subjects = _context.Subjects.AsQueryable();

                    return subjects.Any(sub => req.Any(subjectId => subjectId == sub.Id));
                })
                .WithMessage("The subject ID you entered does not exist. Please try another ID or enter a valid subject ID.");

            RuleFor(e => e.CenterIds)
               .NotEmpty()
               .NotNull()
               .Must(req =>
               {
                   var centers = _context.Centers.AsQueryable();

                   return centers.Any(cen => req.Any(centerId => centerId == cen.Id));
               })
               .WithMessage("The center ID you entered does not exist. Please try another ID or enter a valid center ID.");

            RuleFor(e => e.Date)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow);

            RuleFor(e => e.OpeningDate)
                .NotEmpty()
                .NotNull()
                .LessThan(req => req.ClosingDate)
                .WithMessage("Opening date must be less than the closing date.");

            RuleFor(e => e.ClosingDate)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow)
                .GreaterThan(req => req.OpeningDate)
                .WithMessage("Closing date must be greater than the opening date and current UTC time.");
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
            var examDateExists = await _context.ExamDateSubjects
                .AnyAsync(e =>
                    request.CenterIds.Contains(e.CenterId) &&
                    request.SubjectIds.Contains(e.SubjectId) &&
                    e.ExamDate.Date == request.Date &&
                    e.ExamDate.OpeningDate == request.OpeningDate &&
                    e.ExamDate.ClosingDate == request.ClosingDate,
                        cancellationToken: cancellationToken);

            if (examDateExists)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "An Exam Date with the same data already exists."
                });
            }

            var examDate = _mapper.Map<ExamDateEntity>(request);

            _context.ExamDates.Add(examDate);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the exam date to the database. Please try again later." });
        }
    }
}
