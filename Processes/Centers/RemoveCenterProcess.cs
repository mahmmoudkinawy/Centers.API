namespace Centers.API.Processes.Subjects;
public sealed class RemoveCenterProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid CenterId { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(s => s.CenterId)
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
            var center = await _context.Centers.FindAsync(
                new object?[] { request.CenterId },
                cancellationToken: cancellationToken);

            if (center is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the center with the given ID does not exist. Please check the ID and try again." });
            }

            _context.Centers.Remove(center);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error saving the center to the database. Please try again later." });
        }
    }

}
