namespace Centers.API.Processes.ExamDates;
public sealed class GetExamDatesProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime OpeningDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public ICollection<SubjectResponse> Subjects { get; set; } = new List<SubjectResponse>();
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
            CreateMap<ExamDateEntity, Response>()
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.ExamDateSubjects.Select(es => es.Subject)));
            CreateMap<SubjectEntity, SubjectResponse>();
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
            var query = _context.ExamDates
                .OrderBy(e => e.Id)
                .Include(e => e.ExamDateSubjects)
                    .ThenInclude(es => es.Subject)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }

}
