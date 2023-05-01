namespace Centers.API.Processes.Subjects;
public sealed class UpdateSubjectProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public sealed class Response { }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Request, SubjectEntity>();
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.Name)
                .MaximumLength(100)
                .NotEmpty()
                .NotNull();

            RuleFor(s => s.Description)
                .MaximumLength(3000)
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

            var subjectIdFromRoute = requestRouteQuery!.Values["subjectId"];

            var subjectId = Guid.Parse(subjectIdFromRoute.ToString());

            var subject = await _context.Subjects.FindAsync(
                new object?[] { subjectId },
                cancellationToken: cancellationToken);

            if (subject is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the subject with the given ID does not exist. Please check the ID and try again." });
            }

            _mapper.Map(request, subject);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error updating the subject to the database. Please try again later." });
        }
    }

}
