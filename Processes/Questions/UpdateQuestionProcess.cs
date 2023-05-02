namespace Centers.API.Processes.Questions;
public sealed class UpdateQuestionProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string Text { get; set; }
        public QuestionTypeEnum Type { get; set; }
        public ICollection<ChoiceRequest> Choices { get; set; } = new List<ChoiceRequest>();
        public string AnswerText { get; set; }
    }

    public sealed class ChoiceRequest
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(q => q.Type)
                .NotNull()
                .IsInEnum()
                .Must(t => Enum.IsDefined(typeof(QuestionTypeEnum), t));

            RuleFor(q => q.Text)
                .NotEmpty()
                .NotNull();

            RuleFor(q => q.Choices)
                .NotEmpty()
                .NotNull()
                .When(q => q.Type == QuestionTypeEnum.MultipleChoice)
                .WithMessage("At least one choice is required.")
                .Must(choices =>
                {
                    var correctChoices = choices.Where(c => c.IsCorrect);
                    return correctChoices.Count() == 1;
                })
                .When(q => q.Type == QuestionTypeEnum.MultipleChoice || q.Type == QuestionTypeEnum.TrueFalse)
                .WithMessage("Exactly one choice must be marked as correct.");

            RuleForEach(q => q.Choices)
                .ChildRules(choice =>
                {
                    choice.RuleFor(c => c.Text)
                        .NotEmpty()
                        .NotNull();

                    choice.RuleFor(c => c.IsCorrect)
                        .NotNull();
                })
                .When(q => q.Type == QuestionTypeEnum.MultipleChoice || q.Type == QuestionTypeEnum.TrueFalse)
                .WithMessage("Exactly one choice must be marked as correct.");

            RuleFor(q => q.AnswerText)
                .NotEmpty()
                .NotNull()
                .When(q => q.Type == QuestionTypeEnum.FreeText)
                .WithMessage("Answer text is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(
            CentersDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var requestRouteQuery = _httpContextAccessor.HttpContext?.GetRouteData();

            var questionIdFromRoute = requestRouteQuery!.Values["questionId"];

            var questionId = Guid.Parse(questionIdFromRoute.ToString());

            var question = await _context.Questions
                .Include(c => c.Choices)
                .Include(a => a.Answer)
                .FirstOrDefaultAsync(s => s.Id == questionId && s.OwnerId == currentUserId, cancellationToken: cancellationToken);

            if (question is null)
            {
                return Result<Response>.Failure(
                    new List<string> { "We're sorry, but the question with the given ID does not exist. Please check the ID and try again." });
            }

            question.Type = request.Type;
            question.Text = request.Text;
            question.Choices = Enumerable.Empty<ChoiceEntity>().ToList();

            switch (request.Type)
            {
                case QuestionTypeEnum.MultipleChoice:
                case QuestionTypeEnum.TrueFalse:
                    question.Choices = request.Choices.Select(a => new ChoiceEntity
                    {
                        IsCorrect = a.IsCorrect,
                        Text = a.Text
                    }).ToList();
                    if (question.Answer is not null)
                    {
                        _context.Answers.Remove(question.Answer);
                    }
                    break;
                case QuestionTypeEnum.FreeText:
                    if (question.Answer is null)
                    {
                        question.Answer = new AnswerEntity
                        {
                            Text = request.AnswerText,
                            QuestionId = question.Id
                        };
                    }
                    else
                    {
                        question.Answer.Text = request.AnswerText;
                    }
                    question.Choices = new List<ChoiceEntity>();
                    break;
                default:
                    throw new Exception("You have selected an invalid question type.");
            }

            _context.Questions.Update(question);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Unable to update question. Please try again later."
            });
        }

    }

}
