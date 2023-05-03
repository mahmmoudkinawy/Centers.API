namespace Centers.API.Processes.Questions;
public sealed class GetQuestionsWithAnswersProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        // Maybe will add some filters in the future.
        // Like what type of questions 'MCQ - True/False' you want to return ... etc/
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public string Type { get; set; }
        public ICollection<ChoiceResponse> Choices { get; set; } = new List<ChoiceResponse>();
        public string? AnswerText { get; set; }
    }

    public sealed class ChoiceResponse
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<QuestionEntity, Response>()
                .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Text))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.Answer.Text ?? null));

            CreateMap<ChoiceEntity, ChoiceResponse>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly CentersDbContext _context;

        public Handler(
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            CentersDbContext context)
        {
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _context.Questions
                .Where(q => q.OwnerId == currentUserId)
                .OrderBy(q => q.Id)
                .AsQueryable();

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        }

    }

}
