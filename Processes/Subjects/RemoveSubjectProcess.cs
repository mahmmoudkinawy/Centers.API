namespace Centers.API.Processes.Subjects;
public sealed class RemoveSubjectProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid SubjectId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.SubjectId)
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
            var subject = await _context.Subjects.FindAsync(
                new object?[] { request.SubjectId },
                cancellationToken: cancellationToken);

            if (subject is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the subject with the given ID does not exist. Please check the ID and try again." });
            }

            _context.Subjects.Remove(subject);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the subject to the database. Please try again later." });
        }
    }

}
