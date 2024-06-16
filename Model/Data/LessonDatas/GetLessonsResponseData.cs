public class GetLessonsResponseData
{
    public ApplicationLesson Lesson { get; set; }
    public List<LessonTag> Tags { get; set; }
    public double Rating { get; set; }
    public bool isFav { get; set; }

}