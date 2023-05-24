namespace Centers.API.Entities;
public sealed class SubjectEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    
    public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();
    public ICollection<ExamDateSubject> ExamDateSubjects { get; set; } = new List<ExamDateSubject>();
}
