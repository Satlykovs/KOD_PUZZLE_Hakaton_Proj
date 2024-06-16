using System.ComponentModel.DataAnnotations.Schema;

public class UserFavouriteLesson
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UserId { get; set; }
    public int LessonId { get; set;}

}