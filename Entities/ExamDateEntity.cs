namespace Centers.API.Entities;
public sealed class ExamDateEntity
{
    public Guid Id { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
}
