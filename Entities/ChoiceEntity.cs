namespace Centers.API.Entities;
public sealed class ChoiceEntity
{
    public Guid Id { get; set; }
    public string? Text { get; set; }
    public bool? IsCorrect { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionEntity Question { get; set; }
}
