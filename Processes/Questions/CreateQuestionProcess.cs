namespace Centers.API.Processes.Questions;
public sealed class CreateQuestionProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public string? Text { get; set; }
        public QuestionTypeEnum? Type { get; set; }
        public ICollection<ChoiceRequest>? Choices { get; set; } = new List<ChoiceRequest>();
        public string? AnswerText { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public sealed class ChoiceRequest
    {
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(i => i.ImageFile)
                .Must(image =>
                {
                    if (image is null || image.Length == 0)
                    {
                        return true;
                    }

                    var extension = Path.GetExtension(image.FileName).ToLower();

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Image must be a JPG, PNG, JIF, or JPEG.")
                .Must(imageData => imageData is null || imageData.Length <= 10 * 1024 * 1024)
                .WithMessage($"Image must be smaller than 10MB.");

            RuleFor(q => q.Type)
                .NotNull()
                .Must(t => t != null && Enum.IsDefined(typeof(QuestionTypeEnum), t));

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
                .When(q => q.Type == QuestionTypeEnum.MultipleChoice)
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
                .When(q => q.Type == QuestionTypeEnum.MultipleChoice)
                .WithMessage("Exactly one choice must be marked as correct.");

            RuleFor(q => q.AnswerText)
                .NotNull()
                .When(q => q.Type == QuestionTypeEnum.FreeText)
                .WithMessage("Answer text is required.");
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;

        public Handler(
            CentersDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IPhotoService photoService)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _photoService = photoService ??
                throw new ArgumentNullException(nameof(photoService));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentTeacherId = _httpContextAccessor.HttpContext.User.GetUserById();

            var currentTeacher = await _context.Users
                .FindAsync(new object?[] { currentTeacherId }, cancellationToken: cancellationToken);

            if(currentTeacher.SubjectId == Guid.NewGuid() || currentTeacher.SubjectId is null)
            {
                return Result<Response>.Failure(new List<string> { "You are not assigned to any subjects." });
            }

            var questionEntity = new QuestionEntity
            {
                Id = Guid.NewGuid(),
                OwnerId = currentTeacherId,
                SubjectId = currentTeacher.SubjectId.Value,
                CreatedAt = DateTime.UtcNow,
                Text = request.Text,
                Type = request.Type.ToString()
            };

            switch (request.Type)
            {
                // Will be refactored later on.
                case QuestionTypeEnum.MultipleChoice:
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

            if (request.ImageFile is not null)
            {
                var imageUploadUrl = await _photoService.UploadPhotoAsync(request.ImageFile);

                var imageEntity = new ImageEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = imageUploadUrl,
                    QuestionId = questionEntity.Id
                };
                _context.Images.Add(imageEntity);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Unable to create the question. Please try again later."
            });

        }

    }

}
