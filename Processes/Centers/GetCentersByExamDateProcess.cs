namespace Centers.API.Processes.Centers;
public sealed class GetCentersByExamDateProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public DateTime? ExamDate { get; set; }
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
        public string OwnerName { get; set; }
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

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ExamDateSubject, Response>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Center.Id))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Center.Owner.FirstName} {src.Center.Owner.LastName}"))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Center.Gender))
                .ForMember(dest => dest.Zone, opt => opt.MapFrom(src => src.Center.Zone))
                .ForMember(dest => dest.LocationUrl, opt => opt.MapFrom(src => src.Center.LocationUrl))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Center.Capacity))
                .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => src.Center.IsEnabled))
                .ForMember(dest => dest.LocationUrl, opt => opt.MapFrom(src => src.Center.LocationUrl))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Center.Name))
                .ForMember(dest => dest.Shifts, opt => opt.MapFrom(src => src.Center.Shifts));

            CreateMap<ShiftEntity, ShiftResponse>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.ExamDateSubjects
                .Include(eds => eds.ExamDate)
                .Include(eds => eds.Center)
                    .ThenInclude(c => c.Shifts)
                .Include(eds => eds.Subject)
                .OrderBy(c => c.SubjectId)
                .AsQueryable();

            if (request.ExamDate is not null)
            {
                var targetDate = request.ExamDate.Value.Date;
                query = query.Where(eds => eds.ExamDate.Date.Value.Date == targetDate);
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTrackingWithIdentityResolution(),
                request.PageNumber,
                request.PageSize);
        }
    }
}
