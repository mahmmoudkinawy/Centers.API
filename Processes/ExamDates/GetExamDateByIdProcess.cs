namespace Centers.API.Processes.ExamDates;
public sealed class GetExamDateByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ExamDateId { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;

        public Handler(
            CentersDbContext context,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var examDate = await _context.ExamDates
                .Include(ed => ed.ExamDateSubjects)
                    .ThenInclude(ed => ed.Center)
                        .ThenInclude(edc => edc.Shifts)
                .Where(s => s.Id == request.ExamDateId)
                .Select(ed => ed.ToExamDateWithCenter(request.ExamDateId))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (examDate is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "An exam date with the provided ID does not exist in the database."
                });
            }

            return Result<Response>.Success(_mapper.Map<Response>(examDate));
        }
    }

}

// Manual mapping was chosen for its simplicity and efficiency, considering the specific requirements of this scenario.
// As the project evolves, we can evaluate the possibility of refactoring the mapping approach,
// such as incorporating AutoMapper, to further enhance maintainability and performance.
public static class Mapper
{
    public static Response ToExamDateWithCenter(this ExamDateEntity examDate, Guid examDateId)
    {
        var examDateSubject = examDate.ExamDateSubjects.FirstOrDefault(x => x.ExamDateId == examDateId);
        var center = examDateSubject?.Center;

        return new Response
        {
            Id = examDate.Id,
            ClosingDate = examDate.ClosingDate.Value,
            Date = examDate.Date.Value,
            OpeningDate = examDate.OpeningDate.Value,
            Center = center != null ? new CenterResponse
            {
                Id = center.Id,
                Gender = center.Gender,
                LocationUrl = center.LocationUrl,
                Capacity = center.Capacity.Value,
                IsEnabled = center.IsEnabled.Value,
                Name = center.Name,
                Shifts = center.Shifts.Select(s => new ShiftResponse
                {
                    Id = s.Id,
                    Capacity = s.Capacity.Value,
                    IsEnabled = s.IsEnabled.Value,
                    ShiftEndTime = s.ShiftEndTime.Value,
                    ShiftStartTime = s.ShiftStartTime.Value,
                }).ToList(),
                Zone = center.Zone,
                
            } : null
        };
    }

}

public sealed class Response
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime OpeningDate { get; set; }
    public DateTime ClosingDate { get; set; }
    public CenterResponse Center { get; set; }
}

public sealed class CenterResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Gender { get; set; }
    public string Zone { get; set; }
    public string LocationUrl { get; set; }
    public int Capacity { get; set; }
    public bool IsEnabled { get; set; }
    public ICollection<ShiftResponse> Shifts { get; set; } = new List<ShiftResponse>();
}

public sealed class ShiftResponse
{
    public Guid Id { get; set; }
    public DateTime ShiftStartTime { get; set; }
    public DateTime ShiftEndTime { get; set; }
    public int Capacity { get; set; }
    public bool IsEnabled { get; set; }
}
