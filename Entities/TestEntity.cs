namespace Centers.API.Entities;
public class TestEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public string? Zone { get; set; }
    public string? LocationUrl { get; set; }
    public int? Capacity { get; set; }
    public bool? IsEnabled { get; set; }

    public TestOneEntity TestOne { get; set; }

}

public class TestOneEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Gender { get; set; }
    public string? Zone { get; set; }
    public string? LocationUrl { get; set; }
    public int? Capacity { get; set; }
    public bool? IsEnabled { get; set; }
}