namespace Centers.API.Processes.Shifts;
public sealed class UpdateShiftTimeProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public DateTime ShiftStartTime { get; set; } = DateTime.UtcNow.Date.AddHours(10);
        public TimeSpan ShiftDuration { get; set; } = TimeSpan.FromHours(2);
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
            var shifts = _context.Shifts
                .Where(s => s.ShiftEndTime != null && s.ShiftStartTime != null)
                .AsQueryable();

            if (!await shifts.AnyAsync(cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error updating the shift to the database. Please try again later." });
            }

            foreach (var shift in shifts)
            {
                shift.ShiftStartTime = request.ShiftStartTime;
                shift.ShiftEndTime = shift.ShiftEndTime.Value.Add(request.ShiftDuration);
            }

            await _context.BulkUpdateAsync(shifts, new BulkConfig
            {
                BatchSize = 100
            }, cancellationToken: cancellationToken);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(
                new List<string> { "We're sorry, but there was an error updating the shifts to the database. Please try again later." });
        }


    }
}
