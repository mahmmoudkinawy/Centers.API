namespace Centers.API.Processes.ExamDates;
public sealed class CreateExamDateProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? Date { get; set; }
        public DateTime? OpeningDate { get; set; }
        public DateTime? ClosingDate { get; set; }
    }

    public sealed class Response { }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, ExamDateEntity>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()));
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(e => e.Date)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow);

            RuleFor(e => e.OpeningDate)
                .NotEmpty()
                .NotNull()
                .GreaterThan(DateTime.UtcNow)
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
