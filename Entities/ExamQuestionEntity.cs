namespace Centers.API.Entities;
public sealed class ExamQuestionEntity
{
    public Guid Id { get; set; }
    public DateTime? ExamDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? QuestionsCount { get; set; }
    public bool? IsCanceled { get; set; }
    public string? Types { get; set; } // Maybe replaced later on.

    public Guid QuestionId { get; set; }
    public QuestionEntity Question { get; set; }

    public Guid SubjectId { get; set; }
    public SubjectEntity Subject { get; set; }
}
