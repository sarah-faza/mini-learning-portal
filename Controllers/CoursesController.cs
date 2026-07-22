using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniLearningPlatform.Api.DTOs;
using MiniLearningPlatform.Api.Services;

namespace MiniLearningPlatform.Api.Controllers;

[ApiController]
[Authorize] // Every endpoint here requires a valid JWT.
[Route("api/courses")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    // Pulls the user id out of the JWT claims set by AuthService.
    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/courses  -> dashboard: all courses with enrollment/progress info
    [HttpGet]
    public async Task<ActionResult<List<CourseSummaryDto>>> GetDashboard()
    {
        var courses = await _courseService.GetDashboardAsync(CurrentUserId);
        return Ok(courses);
    }

    // GET /api/courses/{id} -> course detail with ordered lessons (only if enrolled)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CourseDetailDto>> GetCourseDetail(int id)
    {
        var detail = await _courseService.GetCourseDetailAsync(CurrentUserId, id);

        if (detail is null)
        {
            return NotFound(new ApiErrorResponse("Course not found or you are not enrolled in it."));
        }

        return Ok(detail);
    }

    // POST /api/courses/enroll  { "courseId": 2 }
    [HttpPost("enroll")]
    public async Task<IActionResult> Enroll([FromBody] EnrollRequest request)
    {
        var (success, error) = await _courseService.EnrollAsync(CurrentUserId, request.CourseId);

        if (!success)
        {
            return BadRequest(new ApiErrorResponse(error!));
        }

        return Ok(new { message = "Enrolled successfully." });
    }

    // POST /api/courses/lessons/complete  { "lessonId": 5 }
    [HttpPost("lessons/complete")]
    public async Task<IActionResult> CompleteLesson([FromBody] LessonCompleteRequest request)
    {
        var (success, error) = await _courseService.MarkLessonCompleteAsync(CurrentUserId, request.LessonId);

        if (!success)
        {
            return BadRequest(new ApiErrorResponse(error!));
        }

        return Ok(new { message = "Lesson marked as complete." });
    }
}