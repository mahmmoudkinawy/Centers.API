namespace Centers.API.Processes.Shifts;
public sealed class CetShiftByIdProcess
{
    public sealed class Request : IRequest<Result<Response>>
    {
        public Guid ShiftId { get; set; }
    }

    public sealed class Response
    {
        public Guid Id { get; set; }
        public DateTime ShiftStartTime { get; set; }
        public DateTime ShiftEndTime { get; set; }
        public string CenterName { get; set; }
        public Guid CenterId { get; set; }
        public string AdminName { get; set; }
        public Guid AdminId { get; set; }
        public IEnumerable<SubjectResponse> Subjects { get; set; } = new List<SubjectResponse>();
    }

    public sealed class SubjectResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public sealed class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<ShiftEntity, Response>()
                .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => $"{src.Admin.FirstName} {src.Admin.LastName}"))
                .ForMember(dest => dest.CenterName, opt => opt.MapFrom(src => src.Center.Name))
                .ForMember(dest => dest.Subjects, opt => opt.MapFrom(src => src.ShiftSubjects));

            CreateMap<ShiftSubjectEntity, SubjectResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Subject.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Subject.Description));
        }
    }

    public sealed class Handler : IRequestHandler<Request, Result<Response>>
    {
        private readonly IMapper _mapper;
        private readonly CentersDbContext _context;

        public Handler(
            IMapper mapper,
            CentersDbContext context)
        {
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _context = context ??
                throw new ArgumentNullException(nameof(context));
        }

        public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
        {
            var shift = await _context.Shifts
                .ProjectTo<Response>(_mapper.ConfigurationProvider).AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.ShiftId, cancellationToken: cancellationToken);

            if (shift is null)
            {
                return Result<Response>.Failure(new List<string>
                {
                    "The shift with the given ID does not exist."
                });
            }

            return Result<Response>.Success(shift);
        }

    }
}
