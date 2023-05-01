namespace Centers.API.Entities;
public sealed class ShiftSubjectEntity
{
    public Guid Id { get; set; }

    public Guid ShiftId { get; set; }
    public ShiftEntity Shift { get; set; }

    public Guid SubjectId { get; set; }
    public SubjectEntity Subject { get; set; }
}
