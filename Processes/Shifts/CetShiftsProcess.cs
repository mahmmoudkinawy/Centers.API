namespace Centers.API.Processes.Shifts;
public sealed class CetShiftsProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public string CenterName { get; set; }
        public int CenterCapacity { get; set; }
        public Guid CenterId { get; set; }
        public string AdminName { get; set; }
        public Guid AdminId { get; set; }
        public IEnumerable<SubjectResponse> Subjects { get; set; } = new List<SubjectResponse>();
    }

    public sealed class SubjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ShiftEntity, Response>()
                .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => $"{src.Admin.FirstName} {src.Admin.LastName}"))
                .ForMember(dest => dest.CenterName, opt => opt.MapFrom(src => src.Center.Name))
                .ForMember(dest => dest.CenterCapacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.ShiftSubjects));

            CreateMap<ShiftSubjectEntity, SubjectResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subject.Description));
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly IMapper _mapper;
        private readonly CentersDbContext _context;

        public Handler(
            IMapper mapper,
            CentersDbContext context)
        {
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var shift = _context.Shifts
                .OrderBy(s => s.Id)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                shift = shift.Where(s =>
                    s.Admin.FirstName.Contains(keyword) ||
                    s.Admin.LastName.Contains(keyword) ||
                    s.Center.Name.Contains(keyword) ||
                    s.Center.Description.Contains(keyword));
            }

            return await PagedList<Response>.CreateAsync(
                shift.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }

    }


}
