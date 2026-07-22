namespace MiniLearningPlatform.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    // Store only the bcrypt hash, never the plain password.
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}