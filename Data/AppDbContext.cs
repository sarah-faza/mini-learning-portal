using Microsoft.EntityFrameworkCore;
using MiniLearningPlatform.Api.Models;

namespace MiniLearningPlatform.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonCompletion> LessonCompletions => Set<LessonCompletion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // A user can only enroll in a given course once.
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        modelBuilder.Entity<LessonCompletion>()
            .HasOne(lc => lc.User)
            .WithMany()
            .HasForeignKey(lc => lc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LessonCompletion>()
            .HasOne(lc => lc.Lesson)
            .WithMany(l => l.Completions)
            .HasForeignKey(lc => lc.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LessonCompletion>()
            .HasIndex(lc => new { lc.UserId, lc.LessonId })
            .IsUnique();

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // test user. Password is "Password123!".
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "$2a$11$WMrQopuiDWXEaNxCK82ryO44u.1OGLQv0zBtMh2zMmP9/O2Dg/a8W"
        });

        modelBuilder.Entity<Course>().HasData(
            new Course { Id = 1, Title = "Introduction to C#", Description = "Learn the fundamentals of C# programming." },
            new Course { Id = 2, Title = "ASP.NET Core Basics", Description = "Build web APIs using ASP.NET Core." },
            new Course { Id = 3, Title = "SQL Server Essentials", Description = "Learn relational database design and T-SQL." },
            new Course { Id = 4, Title = "Frontend with JavaScript", Description = "Build interactive UIs with vanilla JavaScript." }
        );

        modelBuilder.Entity<Lesson>().HasData(
            // Course 1
            new Lesson { Id = 1, CourseId = 1, Title = "Variables & Types", Content = "Introduction to C# variables and data types.", Order = 1 },
            new Lesson { Id = 2, CourseId = 1, Title = "Control Flow", Content = "If statements, loops, and switch expressions.", Order = 2 },
            new Lesson { Id = 3, CourseId = 1, Title = "Classes & Objects", Content = "Object-oriented basics in C#.", Order = 3 },
            // Course 2
            new Lesson { Id = 4, CourseId = 2, Title = "Minimal APIs", Content = "Building endpoints with Minimal APIs.", Order = 1 },
            new Lesson { Id = 5, CourseId = 2, Title = "Dependency Injection", Content = "Using the built-in DI container.", Order = 2 },
            new Lesson { Id = 6, CourseId = 2, Title = "Middleware", Content = "Understanding the request pipeline.", Order = 3 },
            // Course 3
            new Lesson { Id = 7, CourseId = 3, Title = "Tables & Keys", Content = "Designing tables, primary and foreign keys.", Order = 1 },
            new Lesson { Id = 8, CourseId = 3, Title = "Joins", Content = "INNER, LEFT, and RIGHT joins.", Order = 2 },
            new Lesson { Id = 9, CourseId = 3, Title = "Indexes & Performance", Content = "Improving query performance with indexes.", Order = 3 },
            // Course 4
            new Lesson { Id = 10, CourseId = 4, Title = "DOM Basics", Content = "Selecting and updating DOM elements.", Order = 1 },
            new Lesson { Id = 11, CourseId = 4, Title = "Fetch API", Content = "Making async requests with fetch.", Order = 2 },
            new Lesson { Id = 12, CourseId = 4, Title = "Events", Content = "Handling user interaction with events.", Order = 3 }
        );

        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment { Id = 1, UserId = 1, CourseId = 1, EnrolledAtUtc = seedDate }
        );

        modelBuilder.Entity<LessonCompletion>().HasData(
            new LessonCompletion { Id = 1, UserId = 1, LessonId = 1, CompletedAtUtc = seedDate }
        );
    }
    
}
