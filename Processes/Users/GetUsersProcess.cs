namespace Centers.API.Processes.Users;
public sealed class GetUsersProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        // will include users, pagination, searching, ordering .... etc.

        public string? Keyword { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
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

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
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

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserById();

            var query = _userManager.Users.Where(u => u.Id != currentUserId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                // Must trim the keyword and must make the following code more good and productive.
                // But will do that later.

                var keyword = request.Keyword;
                query = query.Where(u =>
                    u.FirstName.Contains(keyword) ||
                    u.LastName.Contains(keyword) ||
                    u.Email.Contains(keyword) ||
                    u.PhoneNumber.Contains(keyword) ||
                    u.Gender.Contains(keyword) ||
                    u.NationalId.Contains(keyword));
            }

            return await PagedList<Response>
                .CreateAsync(query.ProjectTo<Response>(_mapper.ConfigurationProvider),
                 request.PageNumber,
                 request.PageSize);
        }

    }

}
