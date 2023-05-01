namespace Centers.API.Processes.Images;
public sealed class UserImageUploadProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public IFormFile? Image { get; set; }
    }

    public sealed class Response { }

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(i => i.Image)
                .NotNull()
                .NotEmpty()
                .Must(image =>
                {
                    if (image is null || image.Length == 0)
                    {
                        return true;
                    }

                    var extension = Path.GetExtension(image.FileName).ToLower();
                    return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif";
                })
                .WithMessage("Image must be a JPG, PNG, JIF, or JPEG.")
                .Must(imageData => imageData is null || imageData.Length <= 10 * 1024 * 1024)
                .WithMessage($"Image must be smaller than 10MB.");
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
            var userId = _httpContextAccessor.HttpContext.User.GetUserById();

            var imageUploadUrl = await _photoService.UploadPhotoAsync(request.Image);

            var image = new ImageEntity
            {
                Id = Guid.NewGuid(),
                ImageUrl = imageUploadUrl,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Images.Add(image);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return Result<Response>.Success(new Response { });
            }

            return Result<Response>.Failure(new List<string>
            {
                "Failed to upload user's image, please check the file and try again."
            });
        }

    }

}
