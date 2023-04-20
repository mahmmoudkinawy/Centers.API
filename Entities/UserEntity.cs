namespace Centers.API.Entities;

public sealed class UserEntity : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }

    public Guid? DisabilityId { get; set; }
    public DisabilityEntity? Disability { get; set; }
}
