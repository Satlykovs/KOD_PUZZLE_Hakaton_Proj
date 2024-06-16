using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

public class LessonContext : DbContext
{
    public LessonContext(DbContextOptions<LessonContext> options) : base(options)
    {

    }

    public DbSet<ApplicationLesson> Lessons { get; set; }
    public DbSet<UserFavouriteLesson> UserFavouriteLessons { get; set; }
    public DbSet<LessonText> LessonTexts { get; set; }
    public DbSet<LessonLink> LessonLinks { get; set; }
    public DbSet<LessonPhoto> LessonPhotos { get; set; }
    public DbSet<LessonVideo> LessonVideos { get; set; }
    public DbSet<LessonPresentation> LessonPresentations { get; set; }
    public DbSet<LessonTag> LessonTags { get; set; }
    public DbSet<LessonRating> LessonRatings { get; set; }
    public DbSet<UserRatedLesson> UserRatedLessons{ get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationLesson>().HasKey(l => l.Id);
        modelBuilder.Entity<UserFavouriteLesson>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonText>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonLink>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonPhoto>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonVideo>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonPresentation>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonTag>().HasKey(l => l.Id);
        modelBuilder.Entity<LessonRating>().HasKey(l => l.Id);
        modelBuilder.Entity<UserRatedLesson>().HasKey(l => l.Id);

    
    }
}