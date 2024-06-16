public class GetLessonResponseData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? LessonText { get; set; }
    public List<string>? Links { get; set; }
    public List<string>? PhotoLinks { get; set; }
    public List<string>? VideoLinks { get; set; }
    public List<string>? PresentationsLinks { get; set; }
    public List<string> Tags { get; set; }
}