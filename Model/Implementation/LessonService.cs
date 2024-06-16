public class LessonService : ILessonService
{
    private ILessonRepository _lessonRepository;
    public LessonService(ILessonRepository lessonRepository)
    {
        _lessonRepository = lessonRepository;
    }
    public async Task Add(LessonDataForm dataForm)
    {
        await _lessonRepository.Add(dataForm);
    }
    public async Task Delete(int lessonId)
    {
        await _lessonRepository.Delete(lessonId);
    }
    public async Task Edit(LessonDataForm lessonDataForm)
    {
        await _lessonRepository.Edit(lessonDataForm);
    }
    public async Task Mark(int userId, int lessonId, int mark)
    {
        await _lessonRepository.Mark(userId, lessonId, mark);
    }
    public async Task AddFavourite(int userId, int lessonId)
    {
       await  _lessonRepository.AddFavourite(userId, lessonId);
    }

    public async Task DeleteFavourite(int userId, int lessonId)
    {
        await _lessonRepository.DeleteFavourite(userId, lessonId);
    }

    public async Task<List<GetLessonsResponseData>> GetLessonsNonAuthUser(List<string> tags)
    {
        return await _lessonRepository.GetLessonsNonAuthUser(tags);
    }
    public async Task<List<GetLessonsResponseData>> GetLessonsAuthUser(int userId, List<string> tags)
    {
        return await _lessonRepository.GetLessonsAuthUser(userId, tags);
    }
    public async Task<GetLessonResponseData> GetLesson(int lessonId)
    {
        return await _lessonRepository.GetLesson(lessonId);
    }
}