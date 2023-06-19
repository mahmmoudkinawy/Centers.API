namespace Centers.API.Entities;
public sealed class UserEntity : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? NationalId { get; set; }
    public string? Zone { get; set; }
    public bool IsActive { get; set; }

    public Guid? DisabilityId { get; set; }
    public DisabilityEntity? Disability { get; set; }

    public CenterEntity? Center { get; set; }

    public Guid? SubjectId { get; set; }
    public SubjectEntity? Subject { get; set; }

    public ICollection<ImageEntity> Images { get; set; } = new List<ImageEntity>();
    public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();

    // Later on we will add some properties like makes, subjects ... etc.
}
