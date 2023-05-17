namespace Centers.API.Entities;
public sealed class SubjectEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<ExamDateSubject> ExamDateSubjects { get; set; } = new List<ExamDateSubject>();
}
