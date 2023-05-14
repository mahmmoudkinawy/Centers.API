namespace Centers.API.Entities;
public sealed class ShiftEntity
{
    public Guid Id { get; set; }
    public DateTime? ShiftStartTime { get; set; }
    public DateTime? ShiftEndTime { get; set; }
    public int? Capacity { get; set; }
    public bool? IsEnabled { get; set; }

    public Guid CenterId { get; set; }
    public CenterEntity Center { get; set; }
}
