namespace Centers.API.Entities;
public sealed class QuestionEntity
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public QuestionTypeEnum Type { get; set; }

    public ICollection<ChoiceEntity> Choices { get; set; } = new List<ChoiceEntity>();

    public Guid OwnerId { get; set; }
    public UserEntity Owner { get; set; }

    public Guid? AnswerId { get; set; }
    public AnswerEntity? Answer { get; set; }
}
