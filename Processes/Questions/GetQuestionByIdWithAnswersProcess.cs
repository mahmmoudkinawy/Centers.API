namespace Centers.API.Processes.Questions;
public sealed class GetQuestionByIdWithAnswersProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid QuestionId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Type { get; set; }
        public ICollection<ChoiceResponse> Choices { get; set; } = new List<ChoiceResponse>();
        public string? AnswerText { get; set; }
        public string? ImageUrl { get; set; }
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
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString())) // Will be refactored later on.
                .ForMember(dest => dest.AnswerText, opt => opt.MapFrom(src => src.Answer.Text ?? null))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images.OrderByDescending(i => i.CreatedAt).FirstOrDefault().ImageUrl));

            CreateMap<ChoiceEntity, ChoiceResponse>();
        }
    }
    
    public sealed class Handler : IRequestHandler<Request, Result<Response>>
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

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var question = await _context.Questions
                .Include(q => q.Choices)
                .Include(q => q.Answer)
                .Include(q => q.Images)
                .FirstOrDefaultAsync(q => q.Id == request.QuestionId && q.OwnerId == currentUserId,
                    cancellationToken: cancellationToken);

            if (question is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "Either the question with the provided ID does not exist, or you do not have ownership of it."
                });
            }

            return Result<Response>.Success(_mapper.Map<Response>(question));
        }

    }

}
