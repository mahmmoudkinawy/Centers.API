namespace Centers.API.Processes.Centers;
public sealed class GetCenterWithShiftsByCenterIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid CenterId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Zone { get; set; }
        public string LocationUrl { get; set; }
        public int Capacity { get; set; }
        public bool IsEnabled { get; set; }
        public IEnumerable<ShiftResponse> Shifts { get; set; }
    }

    public sealed class ShiftResponse
    {
        public Guid Id { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public int Capacity { get; set; }
        public bool IsEnabled { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CenterEntity, Response>();
            CreateMap<ShiftEntity, ShiftResponse>();
        }
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
            var center = await _context.Centers
                .Include(s => s.Shifts)
                .FirstOrDefaultAsync(c => c.Id == request.CenterId, 
                    cancellationToken: cancellationToken);

            if (center is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "A center with the provided ID does not exist in the database."
                });
            }

            return Result<Response>.Success(_mapper.Map<Response>(center));
        }
    }
}
