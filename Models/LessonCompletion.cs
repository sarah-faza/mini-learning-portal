namespace MiniLearningPlatform.Api.Models;

public class LessonCompletion
{
     public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public DateTime CompletedAtUtc { get; set; } = DateTime.UtcNow;
}