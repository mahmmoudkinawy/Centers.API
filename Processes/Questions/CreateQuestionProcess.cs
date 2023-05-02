namespace Centers.API.Processes.Questions;
public sealed class CreateQuestionProcess
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

            var questionEntity = new QuestionEntity
            {
                Id = Guid.NewGuid(),
                OwnerId = currentUserId,
                Text = request.Text,
                Type = request.Type
            };

            switch (request.Type)
            {
                // Will be refactored later on.
                case QuestionTypeEnum.MultipleChoice:
                case QuestionTypeEnum.TrueFalse:
                    questionEntity.Choices = request.Choices.Select(a => new ChoiceEntity
                    {
                        Id = Guid.NewGuid(),
                        IsCorrect = a.IsCorrect,
                        Text = a.Text
                    }).ToList();
                    break;
                case QuestionTypeEnum.FreeText:
                    var answerEntity = new AnswerEntity
                    {
                        Id = Guid.NewGuid(),
                        Text = request.AnswerText,
                        QuestionId = questionEntity.Id
                    };
                    questionEntity.Answer = answerEntity;
                    break;
                default:
                    throw new Exception("You have selected an invalid question type.");
            }

            _context.Questions.Add(questionEntity);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Unable to create question. Please try again later."
            });
        }

    }

}
