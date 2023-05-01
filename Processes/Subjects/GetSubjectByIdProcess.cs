namespace Centers.API.Processes.Subjects;
public sealed class GetSubjectByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid SubjectId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<SubjectEntity, Response>();
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
            var subject = await _context.Subjects
                .FindAsync(new object?[] { request.SubjectId }, cancellationToken: cancellationToken);

            if (subject is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "A subject with the provided ID does not exist in the database."
                });
            }

            return Result<Response>.Success(_mapper.Map<Response>(subject));
        }
    }

}
