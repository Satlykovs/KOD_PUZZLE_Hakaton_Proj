using System.ComponentModel.DataAnnotations;

public class ApplicationLesson
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }

}