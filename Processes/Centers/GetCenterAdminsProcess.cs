namespace Centers.API.Processes.Centers;
public sealed class GetCenterAdminsProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<UserEntity, Response>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;

        public Handler(UserManager<UserEntity> userManager,
            CentersDbContext context,
            IMapper mapper)
        {
            _userManager = userManager ??
                throw new ArgumentNullException(nameof(userManager));
            _context = context;
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Where(u => u.IsActive && u.Center.OwnerId == null)
                .Where(u => _context.UserRoles
                    .Join(_context.Roles,
                        userRole => userRole.RoleId,
                        role => role.Id,
                        (userRole, role) => new { UserRole = userRole, Role = role })
                    .Any(joined => joined.UserRole.UserId == u.Id && joined.Role.Name == Constants.Roles.CenterAdmin))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var keyword = request.Keyword.Trim();
                query = query.Where(u =>
                    u.Email.Contains(keyword) ||
                    u.FirstName.Contains(keyword) ||
                    u.LastName.Contains(keyword));
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider).AsQueryable(),
                request.PageNumber,
                request.PageSize);
        }
    }

}