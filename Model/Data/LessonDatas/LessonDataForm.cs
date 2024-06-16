using System.ComponentModel.DataAnnotations;

public class LessonDataForm
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    public string? LessonText { get; set; }
    public List<string>? Links { get; set; }
    public List<IFormFile>? Photos { get; set; }
    public List<IFormFile>? Videos { get; set; }
    public List<IFormFile>? Presentations { get; set; }
    public List<string> Tags { get; set; }

}