using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

public class LessonRepository : ILessonRepository
{
    private readonly LessonContext _lessonContext;
    public LessonRepository(LessonContext lessonContext)
    {
        _lessonContext = lessonContext;
        
    }
    public async Task Add(LessonDataForm dataForm)
    {
        var lesson = new ApplicationLesson
        {
            Id = _lessonContext.Lessons.ToList().Count() + 1,
            Name = dataForm.Name,
            Description = dataForm.Description
        };

        
        string text = dataForm.LessonText;
        LessonText lessonText = new LessonText{LessonId = lesson.Id, Text = text};
        
        List<LessonLink> lessonLinks = new List<LessonLink>();
        if (dataForm.Links != null)
        {
            lessonLinks = dataForm.Links.Select(l => new LessonLink
            {
                LessonId = lesson.Id,
                Link = l
            }).ToList();
        }

        List<LessonPhoto> lessonPhotos = new List<LessonPhoto>();
        if (dataForm.Photos != null)
        {
            foreach (var photo in dataForm.Photos)
            {
                string filePath = $"{Guid.NewGuid()}_{photo.FileName}";
                filePath = $"wwwroot/LessonPhotos/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                lessonPhotos.Add(new LessonPhoto{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonVideo> lessonVideos = new List<LessonVideo>();
        if (dataForm.Videos != null)
        {
            foreach (var video in dataForm.Videos)
            {
                string filePath = $"{Guid.NewGuid()}_{video.FileName}";
                filePath = $"wwwroot/LessonVideos/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await video.CopyToAsync(fileStream);
                }
                lessonVideos.Add(new LessonVideo{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonPresentation> lessonPresentations = new List<LessonPresentation>();
        if (dataForm.Presentations != null)
        {
            foreach (var presentation in dataForm.Presentations)
            {
                string filePath = $"{Guid.NewGuid()}_{presentation.FileName}";
                filePath = $"wwwroot/LessonPresentations/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await presentation.CopyToAsync(fileStream);
                }
                lessonPresentations.Add(new LessonPresentation{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonTag> lessonTags = dataForm.Tags.Select(t => new LessonTag{LessonId = lesson.Id, TagName = t}).ToList();

        _lessonContext.Lessons.Add(lesson);
        _lessonContext.LessonTexts.Add(lessonText);
        if (lessonLinks != null)
        {
            _lessonContext.LessonLinks.AddRange(lessonLinks);
        }
        if (lessonPhotos != null)
        {
            _lessonContext.LessonPhotos.AddRange(lessonPhotos);
        }
        if (lessonVideos != null)
        {
            _lessonContext.LessonVideos.AddRange(lessonVideos);
        }
        if (lessonPresentations != null)
        {
            _lessonContext.LessonPresentations.AddRange(lessonPresentations);
        }
        _lessonContext.LessonTags.AddRange(lessonTags);

        _lessonContext.LessonRatings.Add(new LessonRating
        {
            LessonId = lesson.Id,
            MarkCount = 0,
            Rating = 0
            });

        await _lessonContext.SaveChangesAsync();
    }

    public async Task Delete(int lessonId)
    {
        var lessonToDelete = _lessonContext.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lessonToDelete != null)
        {
            var textToDelete = _lessonContext.LessonTexts.FirstOrDefault(t => t.LessonId == lessonId);
            var linksToDelete = _lessonContext.LessonLinks.Where(l => l.LessonId == lessonId).ToList();
            var photosToDelete = _lessonContext.LessonPhotos.Where(p => p.LessonId == lessonId).ToList();
            var videosToDelete = _lessonContext.LessonVideos.Where(v => v.LessonId == lessonId).ToList();
            var presentationToDelete = _lessonContext.LessonPresentations.Where(p => p.LessonId == lessonId).ToList();
            var tagsToDelete = _lessonContext.LessonTags.Where(t => t.LessonId == lessonId).ToList();
            var lessonRating = _lessonContext.LessonRatings.FirstOrDefault(r => r.LessonId == lessonId);
            var userRatedLessons = _lessonContext.UserRatedLessons.Where(l => l.LessonId == lessonId);
            var UserFavouriteLesson = _lessonContext.UserFavouriteLessons.Where(l => l.LessonId == lessonId);

            foreach (var photo in photosToDelete)
            {
                string filePath = $"./{photo.Path}";
                System.IO.File.Delete(filePath);
            }

            foreach (var video in videosToDelete)
            {
                string filePath = $"./{video.Path}";
                System.IO.File.Delete(filePath);
            }

            foreach (var presentation in presentationToDelete)
            {
                string filePath = $"./{presentation.Path}";
                Console.WriteLine(filePath);
                System.IO.File.Delete(filePath);
            }

            _lessonContext.Lessons.Remove(lessonToDelete);
            _lessonContext.LessonTexts.Remove(textToDelete);
            _lessonContext.LessonLinks.RemoveRange(linksToDelete);
            _lessonContext.LessonPhotos.RemoveRange(photosToDelete);
            _lessonContext.LessonVideos.RemoveRange(videosToDelete);
            _lessonContext.LessonPresentations.RemoveRange(presentationToDelete);
            _lessonContext.LessonTags.RemoveRange(tagsToDelete);
            _lessonContext.LessonRatings.RemoveRange(lessonRating);
            _lessonContext.UserRatedLessons.RemoveRange(userRatedLessons);
            _lessonContext.UserFavouriteLessons.RemoveRange(UserFavouriteLesson);

            await _lessonContext.SaveChangesAsync();
        }
    }

    public async Task Edit(LessonDataForm dataForm)
    {
        var lesson = _lessonContext.Lessons.FirstOrDefault(l => l.Id == dataForm.Id);
        if (lesson != null)
        {
            var lessonId = lesson.Id;

            var textToDelete = _lessonContext.LessonTexts.FirstOrDefault(t => t.LessonId == lessonId);
            var linksToDelete = _lessonContext.LessonLinks.Where(l => l.LessonId == lessonId).ToList();
            var photosToDelete = _lessonContext.LessonPhotos.Where(p => p.LessonId == lessonId).ToList();
            var videosToDelete = _lessonContext.LessonVideos.Where(v => v.LessonId == lessonId).ToList();
            var presentationToDelete = _lessonContext.LessonPresentations.Where(p => p.LessonId == lessonId).ToList();
            foreach (var photo in photosToDelete)
            {
                string filePath = $"./{photo.Path}";
                System.IO.File.Delete(filePath);
            }

            foreach (var video in videosToDelete)
            {
                string filePath = $"./{video.Path}";
                System.IO.File.Delete(filePath);
            }

            foreach (var presentation in presentationToDelete)
            {
                string filePath = $"./{presentation.Path}";
                System.IO.File.Delete(filePath);
            }

            _lessonContext.LessonTexts.Remove(textToDelete);
            _lessonContext.LessonLinks.RemoveRange(linksToDelete);
            _lessonContext.LessonPhotos.RemoveRange(photosToDelete);
            _lessonContext.LessonVideos.RemoveRange(videosToDelete);
            _lessonContext.LessonPresentations.RemoveRange(presentationToDelete);

        string text = dataForm.LessonText;
        LessonText lessonText = new LessonText{LessonId = lesson.Id, Text = text};
        
        List<LessonLink> lessonLinks = new List<LessonLink>();
        if (dataForm.Links != null)
        {
            lessonLinks = dataForm.Links.Select(l => new LessonLink
            {
                LessonId = lesson.Id,
                Link = l
            }).ToList();
        }

        List<LessonPhoto> lessonPhotos = new List<LessonPhoto>();
        if (dataForm.Photos != null)
        {
            foreach (var photo in dataForm.Photos)
            {
                string filePath = $"{Guid.NewGuid()}_{photo.FileName}";
                filePath = $"wwwroot/LessonPhotos/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(fileStream);
                }
                lessonPhotos.Add(new LessonPhoto{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonVideo> lessonVideos = new List<LessonVideo>();
        if (dataForm.Videos != null)
        {
            foreach (var video in dataForm.Videos)
            {
                string filePath = $"{Guid.NewGuid()}_{video.FileName}";
                filePath = $"wwwroot/LessonVideos/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await video.CopyToAsync(fileStream);
                }
                lessonVideos.Add(new LessonVideo{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonPresentation> lessonPresentations = new List<LessonPresentation>();
        if (dataForm.Presentations != null)
        {
            foreach (var presentation in dataForm.Presentations)
            {
                string filePath = $"{Guid.NewGuid()}_{presentation.FileName}";
                filePath = $"wwwroot/LessonPresentations/{filePath}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await presentation.CopyToAsync(fileStream);
                }
                lessonPresentations.Add(new LessonPresentation{LessonId = lesson.Id, Path = filePath});
            }
        }
        List<LessonTag> lessonTags = dataForm.Tags.Select(t => new LessonTag{LessonId = lesson.Id, TagName = t}).ToList();

        _lessonContext.Lessons.Add(lesson);
        _lessonContext.LessonTexts.Add(lessonText);
        if (lessonLinks != null)
        {
            _lessonContext.LessonLinks.AddRange(lessonLinks);
        }
        if (lessonPhotos != null)
        {
            _lessonContext.LessonPhotos.AddRange(lessonPhotos);
        }
        if (lessonVideos != null)
        {
            _lessonContext.LessonVideos.AddRange(lessonVideos);
        }
        if (lessonPresentations != null)
        {
            _lessonContext.LessonPresentations.AddRange(lessonPresentations);
        }
        _lessonContext.LessonTags.AddRange(lessonTags);

        _lessonContext.LessonRatings.Add(new LessonRating
        {
            LessonId = lesson.Id,
            MarkCount = 0,
            Rating = 0
            });

        await _lessonContext.SaveChangesAsync();
        }
    }

    public async Task Mark(int userId, int lessonId, int mark)
    {
        var curMark = _lessonContext.LessonRatings.FirstOrDefault(r => r.LessonId == lessonId);
        
        if (curMark != null)
        {
            if (_lessonContext.UserRatedLessons.FirstOrDefault(r => r.UserId == userId) == null)
            {
                if (curMark.MarkCount == 0)
                {
                    Console.WriteLine(1);
                    curMark.MarkCount = 1;
                    curMark.Rating = Convert.ToDouble(mark);
                }
                else
                {
                    var newRating = curMark.Rating * curMark.MarkCount;
                    newRating += Convert.ToDouble(mark);
                    curMark.MarkCount += 1;
                    curMark.Rating = newRating / curMark.MarkCount;
                }
                _lessonContext.UserRatedLessons.Add(new UserRatedLesson
                {
                    LessonId = lessonId,
                    UserId = userId
                });
                await _lessonContext.SaveChangesAsync();
            }
        }
    }

    public async Task AddFavourite(int userId, int lessonId)
    {
        var favouriteLesson = _lessonContext.UserFavouriteLessons
        .FirstOrDefault(l => l.LessonId == lessonId && l.UserId == userId);
        if (favouriteLesson == null)
        {
            _lessonContext.UserFavouriteLessons.Add(new UserFavouriteLesson
            {
                UserId = userId,
                LessonId = lessonId
            });
        }
        await _lessonContext.SaveChangesAsync();
    }
    public async Task DeleteFavourite(int userId, int lessonId)
    {
        var favouriteLesson = _lessonContext.UserFavouriteLessons
        .FirstOrDefault(l => l.LessonId == lessonId && l.UserId == userId);
        if (favouriteLesson != null)
        {
            _lessonContext.UserFavouriteLessons.Remove(favouriteLesson);
        }
        await _lessonContext.SaveChangesAsync();
    }

    public async Task<List<GetLessonsResponseData>> GetLessonsNonAuthUser(List<string> tags)
    {
        List<ApplicationLesson> lessons = new List<ApplicationLesson>();
        if (tags.Count() == 0)
        {
            lessons = await _lessonContext.Lessons.ToListAsync();
        }
        else
        {
            List<int> lessonIds = await _lessonContext.LessonTags
            .Where(t => tags.Contains(t.TagName)).Select(t => t.LessonId).ToListAsync();
            lessons = await _lessonContext.Lessons.Where(l => lessonIds.Contains(l.Id)).ToListAsync();
        }

        List<GetLessonsResponseData> responseData = lessons.Select(l => new GetLessonsResponseData
        {
            Lesson = l,
            Tags = _lessonContext.LessonTags.Where(t => t.LessonId == l.Id).ToList(),
            Rating = _lessonContext.LessonRatings.FirstOrDefault(r => r.LessonId == l.Id).Rating,
            isFav = false
        }).ToList();
            
            return responseData;
    }
    public async Task<List<GetLessonsResponseData>> GetLessonsAuthUser(int userId, List<string> tags)
    {
        List<ApplicationLesson> lessons = new List<ApplicationLesson>();
        if (tags.Count() == 0)
        {
            lessons = await _lessonContext.Lessons.ToListAsync();
        }
        else
        {
            List<int> lessonIds = _lessonContext.LessonTags
            .Where(t => tags.Contains(t.TagName)).Select(t => t.LessonId).ToList();
            lessons = _lessonContext.Lessons.Where(l => lessonIds.Contains(l.Id)).ToList();
        }

        List<GetLessonsResponseData> responseData = lessons.Select(l => new GetLessonsResponseData
        {
            Lesson = l,
            Tags = _lessonContext.LessonTags.Where(t => t.LessonId == l.Id).ToList(),
            Rating = _lessonContext.LessonRatings.FirstOrDefault(r => r.LessonId == l.Id).Rating,
            isFav = (_lessonContext.UserFavouriteLessons
            .FirstOrDefault(f => f.LessonId == l.Id && f.UserId == userId) != null)
        }).ToList();
            
            return responseData; 
    }


    public async Task<GetLessonResponseData> GetLesson(int lessonId)
    {
        ApplicationLesson lesson = _lessonContext.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson != null)
        {
            string text = _lessonContext.LessonTexts.FirstOrDefault(t => t.LessonId == lessonId).Text;
            List<string> links = await _lessonContext.LessonLinks.Where(l => l.LessonId == lessonId)
            .Select(l => l.Link).ToListAsync();
            List<string> photoLinks = await _lessonContext.LessonPhotos.Where(p => p.LessonId == lessonId)
            .Select(p => p.Path).ToListAsync();
            List<string> videoLinks = await _lessonContext.LessonVideos.Where(v => v.LessonId == lessonId)
            .Select(v => v.Path).ToListAsync();
            List<string> presentationLinks = await _lessonContext.LessonPresentations.Where(p => p.LessonId == lessonId)
            .Select(p => p.Path).ToListAsync();
            List<string> tags = await _lessonContext.LessonTags.Where(t => t.LessonId == lessonId)
            .Select(t => t.TagName).ToListAsync();
            
            var response = new GetLessonResponseData
            {
                Id = lessonId,
                Name = lesson.Name,
                Description = lesson.Description,
                LessonText = text,
                Links = links,
                PhotoLinks = photoLinks,
                VideoLinks = videoLinks,
                PresentationsLinks = presentationLinks,
                Tags = tags
            };
            
            return response;
        }
        return null;
    }


}