namespace Centers.API.Processes.ExamDates;
public sealed class GetExamDateByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ExamDateId { get; set; }
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
                .FindAsync(new object?[] { request.ExamDateId }, cancellationToken: cancellationToken);

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
