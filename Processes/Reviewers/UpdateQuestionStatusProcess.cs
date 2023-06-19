namespace Centers.API.Processes.Reviewers;
public sealed class UpdateQuestionStatusProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public bool? IsApproved { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.IsApproved)
                .NotNull()
                .NotEmpty();
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
            var questionIdFromRoute = _httpContextAccessor.HttpContext?.GetRouteValue("questionId");

            var questionId = Guid.Parse(questionIdFromRoute.ToString());

            var question = await _context.Questions
                .FindAsync(new object?[] { questionId, cancellationToken },
                    cancellationToken: cancellationToken);

            if (question is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We apologize, but either the given ID does not exist or you do not own the associated question. Please double-check the ID and try again." });
            }

            question.IsApproved = request.IsApproved;
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
            new List<string> { "We're sorry, but there was an error updating the question status. Please try again later." });
        }

    }

}
