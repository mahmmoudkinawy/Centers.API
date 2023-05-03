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
                .IsInEnum()
                .Must(t => Enum.IsDefined(typeof(QuestionTypeEnum), t));

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
                .Must(file => file.Length > 0)
                .WithMessage("File is empty.")
                .Must(file => file is not null && file.Length <= 30 * 1024 * 1024)
                .WithMessage("File size exceeds the limit of 30 MB.");

        }
    }

    public sealed class QuestionToCreate
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

        public Handler(
            IServiceScopeFactory serviceScopeFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceScopeFactory = serviceScopeFactory ??
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            using var stream = request.File.OpenReadStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var csv = new CsvReader(reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    BadDataFound = null
                });

            var questions = new List<QuestionEntity>();
            foreach (var questionToCreate in csv.GetRecords<QuestionToCreate>())
            {
                var questionEntity = new QuestionEntity
                {
                    Id = Guid.NewGuid(),
                    OwnerId = currentUserId,
                    Text = questionToCreate.QuestionText,
                    Answer = new AnswerEntity
                    {
                        Id = Guid.NewGuid(),
                        Text = questionToCreate.AnswerText
                    },
                    Type = request.Type
                };
                questionEntity.Answer.QuestionId = questionEntity.Id;
                questions.Add(questionEntity);
            }

            //ThreadPool.QueueUserWorkItem(async _ =>
            //{
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CentersDbContext>();

            var qu = questions.AsEnumerable();

            context.Questions.AddRange(qu);
            await context.SaveChangesAsync();

            await context.Questions.BulkInsertAsync(qu);

            await context.BulkSaveChangesAsync();

            //await context.BulkMergeAsync(qu);

            //await context.SaveChangesAsync();
            //});

            return Result<Response>.Success(new Response { });
        }

    }

}
