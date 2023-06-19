namespace Centers.API.Processes.Reviewers;
public sealed class GetQuestionsForExamBySubjectIdProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Guid? SubjectId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<QuestionEntity, Response>().ReverseMap();
        }
    }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator(CentersDbContext context)
        {
            RuleFor(s => s.SubjectId)
                .NotEmpty()
                .NotNull()
                .Must(subjectId => context.ExamQuestions.Any(s => s.SubjectId == subjectId))
                .WithMessage("No exam questions with the given Subject Id");
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;

        public Handler(CentersDbContext context, IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.ExamQuestions
                .Where(e => !e.IsCanceled.Value && e.SubjectId == request.SubjectId)
                .OrderBy(e => e.Id)
                .Select(e => e.Question)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking(),
                request.PageNumber,
                request.PageSize);
        }
    }

}
