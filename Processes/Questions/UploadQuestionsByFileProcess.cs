namespace Centers.API.Processes.Questions;
public sealed class UploadQuestionsByFileProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public QuestionTypeEnum Type { get; set; }
        public IFormFile File { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(q => q.Type)
                .NotNull()
                .Must(t => t != null && Enum.IsDefined(typeof(QuestionTypeEnum), t));

            RuleFor(q => q.File)
                .NotNull()
                .NotEmpty()
                .When(q => q.File is not null)
                .Must(file =>
                {
                    var allowedExtensions = new[] { ".csv", ".xlsx" };
                    var extension = Path.GetExtension(file.FileName);

                    return allowedExtensions.Contains(extension);
                })
                .WithMessage("Only CSV and XLSX files are allowed.")
                .When(q => q.File is not null)
                .Must(file => file != null && file.Length > 0)
                .WithMessage("File is empty.")
                .Must(file => file is not null && file.Length <= 30 * 1024 * 1024)
                .WithMessage("File size exceeds the limit of 30 MB.");
        }
    }

    public sealed class QuestionToBeCreateFromCsvFile
    {
        // will only work for Type 2 and it's 'FreeText' until now.

        [Index(0)]
        public string QuestionText { get; set; }

        [Index(1)]
        public string AnswerText { get; set; }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CentersDbContext _context;

        public Handler(
            IServiceScopeFactory serviceScopeFactory,
            IHttpContextAccessor httpContextAccessor,
            CentersDbContext context)
        {
            _serviceScopeFactory = serviceScopeFactory ??
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentTeacherId = _httpContextAccessor.HttpContext.User.GetUserById();
            var currentTeacher = await _context.Users.FindAsync(new object?[] { currentTeacherId }, cancellationToken: cancellationToken);

            using var stream = request.File.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    BadDataFound = null
                });

            var questions = new List<QuestionEntity>();
            var answers = new List<AnswerEntity>();
            foreach (var questionToCreate in csv.GetRecords<QuestionToBeCreateFromCsvFile>())
            {
                var questionIdToCreate = Guid.NewGuid();

                var questionEntity = new QuestionEntity
                {
                    Id = questionIdToCreate,
                    OwnerId = currentTeacherId,
                    SubjectId = currentTeacher.SubjectId.Value,
                    CreatedAt = DateTime.UtcNow,
                    Type = request.Type.ToString(),
                };

                switch (request.Type)
                {
                    // Later on will continue this logic.
                    case QuestionTypeEnum.MultipleChoice:
                        return Result<Response>.Failure(new List<string>
                        {
                            "The logic for Multiple Choice Questions and True False does not implemented yet."
                        });
                    case QuestionTypeEnum.FreeText:
                        var answerIdToCreate = Guid.NewGuid();
                        var answerEntity = new AnswerEntity
                        {
                            Id = answerIdToCreate,
                            Text = questionToCreate.AnswerText,
                            QuestionId = questionIdToCreate
                        };
                        questionEntity.Text = questionToCreate.QuestionText;
                        questionEntity.AnswerId = answerIdToCreate;
                        answers.Add(answerEntity);
                        break;
                    default:
                        throw new Exception("You have selected an invalid question type.");
                }

                questions.Add(questionEntity);
            }

            // To ensure that all the steps are completed together or none of them at all, it is necessary for it to be treated as a transaction.
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _context.Answers.AddRange(answers);
            _context.Questions.AddRange(questions);
            await _context.BulkSaveChangesAsync(cancellationToken: cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return Result<Response>.Success(new Response { });
        }

    }


}
