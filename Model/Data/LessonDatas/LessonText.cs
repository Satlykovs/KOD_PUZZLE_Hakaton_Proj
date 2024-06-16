using System.ComponentModel.DataAnnotations.Schema;

public class LessonText
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string Text { get; set; }
}