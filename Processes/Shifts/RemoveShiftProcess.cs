namespace Centers.API.Processes.Shifts;
public sealed class RemoveShiftProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ShiftId { get; set; }
    }

    public sealed class Response { }

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
            var shift = await _context.Shifts
                .FindAsync(new object?[] { request.ShiftId }, cancellationToken: cancellationToken);

            if (shift is null)
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but the shift with the given ID does not exist. Please check the ID and try again." });
            }

            _context.Shifts.Remove(shift);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error removing the shift from the database. Please try again later." });

        }

    }
}
