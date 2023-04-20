namespace Centers.API.Processes.Users;
public sealed class GetUsersProcess
{
    public sealed class Request : IRequest<IReadOnlyList<Response>>
    {
        // will include users, pagination, searching, ordering .... etc.
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(u => $"{u.FirstName} {u.LastName}"));
        }
    }

    public sealed class Handler : IRequestHandler<Request, IReadOnlyList<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public Handler(
            UserManager<UserEntity> userManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _httpContextAccessor = httpContextAccessor ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IReadOnlyList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _userManager.Users.Where(u => u.Id != currentUserId).AsQueryable();

            var users = await query.ProjectTo<Response>(_mapper.ConfigurationProvider).ToListAsync();

            return users;
        }

    }

}
