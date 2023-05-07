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
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ExamDateEntity, Response>();
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
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }

}
