namespace Centers.API.Entities;
public sealed class ExamDateSubject
{
    public Guid ExamDateId { get; set; }
    public ExamDateEntity ExamDate { get; set; }

    public Guid SubjectId { get; set; }
    public SubjectEntity Subject { get; set; }

    public Guid CenterId { get; set; }
    public CenterEntity Center { get; set; }
}