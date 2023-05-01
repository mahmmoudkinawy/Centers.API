namespace Centers.API.Entities;
public sealed class ShiftEntity
{
    public Guid Id { get; set; }
    public DateTime? ShiftStartTime { get; set; }
    public DateTime? ShiftEndTime { get; set; }

    public Guid CenterId { get; set; }
    public CenterEntity Center { get; set; }

    public Guid AdminId { get; set; }
    public UserEntity Admin { get; set; }

    // Relationship until now from one side
    public ICollection<ShiftSubjectEntity> ShiftSubjects { get; set; } = new List<ShiftSubjectEntity>();
}
