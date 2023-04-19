namespace Centers.API.Entities;

[Table("Disabilities")]
public sealed class DisabilityEntity
{
    public Guid Id { get; set; }
    public bool HasDisability { get; set; }
    public string? ImageUrl { get; set; }

    public Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; }
}
