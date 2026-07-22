# GitHub Report — Mini Learning Portal

## Time Tracked vs Planned
| Phase          | Planned | Actual         | Notes                                   |
|----------------|---------|----------------|------------------------------------------|
| Planning       | 2h      | 1h50m          | Schema, architecture, API design         |
| Implementation | 6h      | 5h45m          | See milestone breakdown below         |

**Planning start:** 10:00 AM   **Planning ended:** 11:50 AM   **Implementation started:** 12:00 PM   **Implementation ended:** 5:45 PM

### Implementation Milestones
1. **Database & Models** — EF Core models (`User`, `Course`, `Lesson`, `Enrollment`, `LessonCompletion`), `AppDbContext` with relationships, unique indexes, and seed data; migration created and applied against SQL Server Express.
2. **Auth & Business Logic** — `AuthService` (BCrypt password verification + JWT issuance), `CourseService` (dashboard, course detail, enrollment, lesson completion, progress/status calculation).
3. **API Layer** — `AuthController` and `CoursesController`, wired up in `Program.cs` with JWT authentication, CORS, and centralized error handling.
4. **Frontend** — `index.html` (login / dashboard / course detail views with Tailwind) and `app.js` (Fetch-based API calls, no full page reloads).
5. **Testing & Verification** — Manual end-to-end testing via PowerShell (`Invoke-RestMethod`) and in-browser testing (login → dashboard → enroll → complete lesson → progress update).

## Core Features Completed
- [x] Login with username/password, JWT-based auth, clear error on bad credentials
- [x] Course dashboard: my courses vs. available courses
- [x] Enrollment (persisted to SQL Server, no reload)
- [x] Course detail view with ordered lessons
- [x] Mark lesson complete (persisted, idempotent)
- [x] Dynamic progress % and status (Not Started / In Progress / Completed)
- [x] Seed data: 1 user, 4 courses, 3 lessons each, 1 pre-enrolled course with 1 completed lesson

## Remaining Items / Trade-offs Due to Timebox
- No logout token revocation/blacklist (JWT simply expires client-side on logout)
- No password reset / registration flow (out of scope per requirements)
- No automated tests (unit/integration) — manual testing only, due to the timebox
- Minimal client-side validation; relies on backend validation for correctness

## Known Limitations & Future Enhancements
- Add refresh tokens for longer sessions without re-login
- Add a course-management admin UI (currently seed-only, per requirements)
- Add unit tests for `CourseService` progress/status calculations
- Add pagination if the course catalog grows significantly
- Move the JWT signing key out of `appsettings.json` into a secrets manager for production