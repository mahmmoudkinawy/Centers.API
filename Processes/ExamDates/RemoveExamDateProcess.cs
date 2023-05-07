namespace Centers.API.Processes.ExamDates;
public sealed class RemoveExamDateProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ExamDateId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.ExamDateId)
                .NotEmpty()
                .NotNull();
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;

        public Handler(
            CentersDbContext context)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var examDate = await _context.ExamDates.FindAsync(
                new object?[] { request.ExamDateId },
                cancellationToken: cancellationToken);

            if (examDate is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the exam date with the given ID does not exist. Please check the ID and try again." });
            }

            _context.ExamDates.Remove(examDate);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the exam date to the database. Please try again later." });
        }
    }
}
