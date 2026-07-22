namespace MiniLearningPlatform.Api.Models;

public class Lesson
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    // Determines display order within the course.
    public int Order { get; set; }

    public ICollection<LessonCompletion> Completions { get; set; } = new List<LessonCompletion>();
}