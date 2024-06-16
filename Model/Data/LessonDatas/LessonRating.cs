using System.ComponentModel.DataAnnotations.Schema;

public class LessonRating
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int MarkCount { get; set; }
    public double Rating { get; set; }
}