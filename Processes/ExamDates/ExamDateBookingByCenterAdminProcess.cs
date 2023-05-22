namespace Centers.API.Processes.ExamDates;
public sealed class ExamDateBookingByCenterAdminProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ExamDateId { get; set; }
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
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            if (!await _context.ExamDates.AnyAsync(ed => ed.Id == request.ExamDateId,
                    cancellationToken: cancellationToken))
            {
                return Result<Response>.Failure(new List<string>
                {
                    "No exam date exists with the given ID."
                });
            }

            var center = await _context.Centers
                .FirstOrDefaultAsync(c => c.OwnerId == currentUserId,
                    cancellationToken: cancellationToken);

            if (center is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Unfortunately, you do not have administrative privileges for any center, which means you cannot proceed further."
                });
            }

            var examDateExists = await _context.ExamDateSubjects
                .AnyAsync(eds => eds.ExamDateId == request.ExamDateId &&
                                 eds.CenterId == center.Id,
                              cancellationToken: cancellationToken);

            if (examDateExists)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Your center has already been assigned this exam date."
                });
            }

            var examDateWithSubjects = await _context.ExamDateSubjects
                .Where(eds => eds.ExamDateId == request.ExamDateId)
                .ToListAsync(cancellationToken: cancellationToken);

            if (!examDateWithSubjects.Any())
            {
                return Result<Response>.Failure(new List<string>
                {
                    "No subjects are available for this exam date."
                });
            }

            foreach (var examDateSubject in examDateWithSubjects)
            {
                var examDate = new ExamDateSubject
                {
                    ExamDateId = request.ExamDateId,
                    CenterId = center.Id,
                    SubjectId = examDateSubject.SubjectId
                };

                _context.ExamDateSubjects.Add(examDate);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Unfortunately, an error occurred while attempting to add the exam date subject to the database. Please try again later."
            });

        }

    }

}
