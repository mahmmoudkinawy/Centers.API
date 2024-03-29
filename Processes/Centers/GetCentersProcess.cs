﻿namespace Centers.API.Processes.Centers;
public sealed class GetCentersProcess
{
    public sealed class Request : IRequest<PagedList<Response>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }

        // Need to add some filters like opening date ... etc.
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Zone { get; set; }
        public string LocationUrl { get; set; }
        public int Capacity { get; set; }
        public bool IsEnabled { get; set; }
        public string OwnerName { get; set; }
        public ICollection<ShiftResponse> Shifts { get; set; } = new List<ShiftResponse>();
    }

    public sealed class ShiftResponse
    {
        public Guid Id { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public int Capacity { get; set; }
        public bool IsEnabled { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CenterEntity, Response>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => $"{src.Owner.FirstName} {src.Owner.LastName}"));
            CreateMap<ShiftEntity, ShiftResponse>();
        }
    }

    public sealed class Handler : IRequestHandler<Request, PagedList<Response>>
    {
        private readonly CentersDbContext _context;
        private readonly IMapper _mapper;

        public Handler(
            CentersDbContext context,
            IMapper mapper)
        {
            _context = context ??
                throw new ArgumentNullException(nameof(context));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<PagedList<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var query = _context.Centers
                .OrderBy(c => c.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(s =>
                    s.Name.Contains(request.Keyword) ||
                    s.Gender.Contains(request.Keyword) ||
                    s.Zone.Contains(request.Keyword) ||
                    s.LocationUrl.Contains(request.Keyword));
            }

            return await PagedList<Response>.CreateAsync(
                query.ProjectTo<Response>(_mapper.ConfigurationProvider),
                request.PageNumber,
                request.PageSize);
        }
    }

}
