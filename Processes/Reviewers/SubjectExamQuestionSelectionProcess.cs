namespace Centers.API.Processes.Reviewers;
public sealed class SubjectExamQuestionSelectionProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public IEnumerable<string>? Types { get; set; }
        public int? QuestionsCount { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(q => q.From)
                .NotEmpty()
                .NotNull();

            RuleFor(q => q.To)
                .NotEmpty()
                .NotNull();

            RuleFor(q => q.From)
                .NotEqual(q => q.To);

            RuleFor(q => q.QuestionsCount)
                .NotEmpty()
                .NotNull()
                .GreaterThan(0)
                .LessThanOrEqualTo(500);

            RuleFor(q => q.Types)
                .NotEmpty()
                .NotNull()
                .Must(types =>
                {
                    var allowedTypes = new[] { "FreeText", "MCQ" };

                    return types.All(t => allowedTypes.Contains(t));
                });
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(CentersDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var subjectIdFromRoute = requestRouteQuery!.Values["subjectId"];

            var subjectId = Guid.Parse(subjectIdFromRoute.ToString());

            var subject = await _context.Subjects
                .FindAsync(new object?[] { subjectId },
                    cancellationToken: cancellationToken);

            if (subject is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "We couldn't find a subject associated with the given ID."
                });
            }

            var query = _context.Questions
                .Where(q =>
                            q.SubjectId == subject.Id &&
                            q.IsApproved.Value &&
                            q.CreatedAt >= request.From &&
                            q.CreatedAt <= request.To &&
                            request.Types.Contains(q.Type) &&
                            _context.ExamDateSubjects
                                .Any(eds =>
                                            eds.SubjectId == q.SubjectId &&
                                            eds.ExamDate.ClosingDate >= DateTime.UtcNow))
                .Take(request.QuestionsCount.Value)
                .OrderBy(q => q.Id)
                .AsQueryable();

            return null;
        }

    }

}
