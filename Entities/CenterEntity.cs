namespace Centers.API.Entities;
public sealed class CenterEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? Description { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }

    public ICollection<ShiftEntity> Shifts { get; set; } = new List<ShiftEntity>();

    // need to add more properties such Employees, Students, Subjects, Owner of this center .... etc
}
