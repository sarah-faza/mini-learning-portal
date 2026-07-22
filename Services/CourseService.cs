using Microsoft.EntityFrameworkCore;
using MiniLearningPlatform.Api.Data;
using MiniLearningPlatform.Api.DTOs;

namespace MiniLearningPlatform.Api.Services;

public class CourseService : ICourseService
{
    private readonly AppDbContext _db;

    public CourseService(AppDbContext db)
    {
        _db = db;
    }

    private static (int Percentage, string Status) ComputeProgress(int totalLessons, int completedLessons)
    {
        if (totalLessons == 0) return (0, "Not Started");

        var percentage = (int)Math.Round(completedLessons * 100.0 / totalLessons);

        var status = completedLessons == 0
            ? "Not Started"
            : completedLessons >= totalLessons
                ? "Completed"
                : "In Progress";

        return (percentage, status);
    }

    public async Task<List<CourseSummaryDto>> GetDashboardAsync(int userId)
    {
        var enrolledCourseIds = await _db.Enrollments
            .Where(e => e.UserId == userId)
            .Select(e => e.CourseId)
            .ToListAsync();

        var completedLessonIds = await _db.LessonCompletions
            .Where(lc => lc.UserId == userId)
            .Select(lc => lc.LessonId)
            .ToListAsync();

        var courses = await _db.Courses
            .Include(c => c.Lessons)
            .AsNoTracking()
            .ToListAsync();

        var result = new List<CourseSummaryDto>();

        foreach (var course in courses)
        {
            var isEnrolled = enrolledCourseIds.Contains(course.Id);
            var totalLessons = course.Lessons.Count;
            var completedCount = course.Lessons.Count(l => completedLessonIds.Contains(l.Id));

            var (percentage, status) = isEnrolled
                ? ComputeProgress(totalLessons, completedCount)
                : (0, "Not Started");

            result.Add(new CourseSummaryDto(
                course.Id,
                course.Title,
                course.Description,
                totalLessons,
                isEnrolled,
                isEnrolled ? completedCount : 0,
                percentage,
                status
            ));
        }

        return result;
    }

    public async Task<CourseDetailDto?> GetCourseDetailAsync(int userId, int courseId)
    {
        var course = await _db.Courses
            .Include(c => c.Lessons)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId);

        if (course is null) return null;

        var isEnrolled = await _db.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);

        if (!isEnrolled) return null;

        var completedLessonIds = await _db.LessonCompletions
            .Where(lc => lc.UserId == userId && lc.Lesson!.CourseId == courseId)
            .Select(lc => lc.LessonId)
            .ToListAsync();

        var totalLessons = course.Lessons.Count;
        var (percentage, status) = ComputeProgress(totalLessons, completedLessonIds.Count);

        var lessonDtos = course.Lessons
            .OrderBy(l => l.Order)
            .Select(l => new LessonDto(l.Id, l.Title, l.Content, l.Order, completedLessonIds.Contains(l.Id)))
            .ToList();

        return new CourseDetailDto(course.Id, course.Title, course.Description, percentage, status, lessonDtos);
    }

    public async Task<(bool Success, string? Error)> EnrollAsync(int userId, int courseId)
    {
        var courseExists = await _db.Courses.AnyAsync(c => c.Id == courseId);
        if (!courseExists) return (false, "Course not found.");

        var alreadyEnrolled = await _db.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        if (alreadyEnrolled) return (false, "Already enrolled in this course.");

        _db.Enrollments.Add(new Models.Enrollment { UserId = userId, CourseId = courseId });
        await _db.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? Error)> MarkLessonCompleteAsync(int userId, int lessonId)
    {
        var lesson = await _db.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
        if (lesson is null) return (false, "Lesson not found.");

        var isEnrolled = await _db.Enrollments
            .AnyAsync(e => e.UserId == userId && e.CourseId == lesson.CourseId);
        if (!isEnrolled) return (false, "You must enroll in this course before completing lessons.");

        var alreadyCompleted = await _db.LessonCompletions
            .AnyAsync(lc => lc.UserId == userId && lc.LessonId == lessonId);
        if (alreadyCompleted) return (true, null);

        _db.LessonCompletions.Add(new Models.LessonCompletion { UserId = userId, LessonId = lessonId });
        await _db.SaveChangesAsync();

        return (true, null);
    }
}