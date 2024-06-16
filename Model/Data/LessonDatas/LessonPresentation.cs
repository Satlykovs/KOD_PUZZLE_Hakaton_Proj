using System.ComponentModel.DataAnnotations.Schema;

public class LessonPresentation
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Path { get; set; }
}