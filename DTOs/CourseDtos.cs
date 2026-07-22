namespace MiniLearningPlatform.Api.DTOs;

public record CourseSummaryDto(
    int Id,
    string Title,
    string Description,
    int LessonCount,
    bool IsEnrolled,
    int CompletedLessons,
    int ProgressPercentage,
    string Status // "Not Started" | "In Progress" | "Completed"
);

public record CourseDetailDto(
    int Id,
    string Title,
    string Description,
    int ProgressPercentage,
    string Status,
    List<LessonDto> Lessons
);

public record LessonDto(
    int Id,
    string Title,
    string Content,
    int Order,
    bool IsCompleted
);

public record EnrollRequest(int CourseId);

public record LessonCompleteRequest(int LessonId);

public record ApiErrorResponse(string Message);