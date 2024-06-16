using System.ComponentModel.DataAnnotations.Schema;

public class LessonLink
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int LessonId { get; set; }
    
    public string Link  { get; set; }
}