using MiniLearningPlatform.Api.DTOs;

namespace MiniLearningPlatform.Api.Services;

public interface ICourseService
{
    Task<List<CourseSummaryDto>> GetDashboardAsync(int userId);
    Task<CourseDetailDto?> GetCourseDetailAsync(int userId, int courseId);
    Task<(bool Success, string? Error)> EnrollAsync(int userId, int courseId);
    Task<(bool Success, string? Error)> MarkLessonCompleteAsync(int userId, int lessonId);
}