namespace Centers.API.Entities;
public sealed class AnswerEntity
{
    public Guid Id { get; set; }
    public string? Text { get; set; }

    public Guid QuestionId { get; set; }
    public QuestionEntity Question { get; set; }
}