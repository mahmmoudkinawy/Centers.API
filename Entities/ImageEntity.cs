namespace Centers.API.Entities;
public sealed class ImageEntity
{
    public Guid Id { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? CreatedAt { get; set; }

    public Guid? UserId { get; set; }
    public UserEntity? User { get; set; }

    public Guid? QuestionId { get; set; }
    public QuestionEntity? Question { get; set; }
}