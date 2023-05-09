namespace Centers.API.Processes.Users;
public class GetCurrentUserProfileProcess
{
    public sealed class Request : IRequest<Response> { }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Zone { get; set; }
        public string UserImageUrl { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"))
                .ForMember(dest => dest.UserImageUrl, opt => opt.MapFrom(u => u.Images.MaxBy(i => i.CreatedAt).ImageUrl));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Response>
    {
        private readonly CentersDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            CentersDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var user = await _context.Users
                .Include(i => i.Images)
                .FirstOrDefaultAsync(u => u.Id == currentUserId,
                    cancellationToken: cancellationToken);

            return _mapper.Map<Response>(user);
        }

    }
}
