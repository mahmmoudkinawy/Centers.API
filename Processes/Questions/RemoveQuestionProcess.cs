namespace Centers.API.Processes.Questions;
public sealed class RemoveQuestionProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid QuestionId { get; set; }
    }

    public sealed class Response { }

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
            var currentTeacherId = _httpContextAccessor.HttpContext.User.GetUserById();

            var currentTeacher = await _context.Users
                .FindAsync(new object?[] { currentTeacherId }, cancellationToken: cancellationToken);

            // I know that i can avoid all of that if i used Cascade 
            // But I need that.
            var question = await _context.Questions
                .Include(c => c.Answer)
                .Include(q => q.Images)
                .FirstOrDefaultAsync(q =>
                    q.Id == request.QuestionId &&
                    q.OwnerId == currentTeacherId &&
                    q.SubjectId == currentTeacher.SubjectId, cancellationToken: cancellationToken);

            if (question is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We apologize, but either the given ID does not exist or you do not own the associated question. Please double-check the ID and try again." });
            }

            _context.Questions.Remove(question);

            if (question.Answer is not null)
            {
                _context.Answers.Remove(question.Answer);
            }

            if (question.Images.Any())
            {
                _context.Images.RemoveRange(question.Images);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error removing the question from the database. Please try again later." });

        }

    }
}
