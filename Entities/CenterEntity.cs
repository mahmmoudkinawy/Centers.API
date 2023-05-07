namespace Centers.API.Entities;
public sealed class CenterEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public string? Zone { get; set; }
    public string? LocationUrl { get; set; }
    public int? Capacity { get; set; }
    public bool? IsEnabled { get; set; }
}
