# Mini Learning Portal

A small full-stack learning portal: sign in, view courses, enroll, complete lessons, and track progress.

## Tech Stack
- **Backend:** C# / .NET 10 Web API (Controller → Service → EF Core pattern)
- **Database:** SQL Server (EF Core Migrations + Seed Data)
- **Frontend:** HTML5, Tailwind CSS (CDN), vanilla JavaScript (Fetch API)

## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (SQL Server Express, LocalDB, or SQL Server in Docker)
- A code editor (Visual Studio Code recommended)
- Git

## 1. Configure the database connection
Edit `backend/MiniLearningPlatform.Api/appsettings.json` and update `ConnectionStrings:DefaultConnection`
to point at your SQL Server instance. Example (SQL Server Express, Windows Authentication):

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=MiniLearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

## 2. Restore packages
```bash
cd backend/MiniLearningPlatform.Api
dotnet restore
```

## 3. Create and apply the EF Core migration
```bash
dotnet tool install --global dotnet-ef   # one-time, if not already installed
dotnet ef migrations add InitialCreate
dotnet ef database update
```
> The app also calls `db.Database.Migrate()` on startup, so `dotnet ef database update`
> is optional once the migration exists — running the app will apply it automatically.

## 4. Run the API
```bash
dotnet run
```
The API starts on the URL shown in the console (e.g. `http://localhost:5126`).

## 5. Run the frontend
The frontend is static (no build step). Open `frontend/index.html` with the
**Live Server** extension in VS Code (right-click → "Open with Live Server").

If your API runs on a different port than `5126`, update `API_BASE` at the
top of `frontend/app.js` to match.

## Test Credentials
| Username | Password       |
|----------|----------------|
| testuser | Password123!   |

`testuser` is pre-enrolled in **Introduction to C#**, with the first lesson already completed.

## Project Structure
```
backend/MiniLearningPlatform.Api/
  Controllers/     -> AuthController, CoursesController
  Services/        -> AuthService, CourseService (business logic)
  Data/            -> AppDbContext (EF Core, relationships, seed data)
  Models/          -> User, Course, Lesson, Enrollment, LessonCompletion
  DTOs/            -> Request/response objects (no cyclic JSON)
  Migrations/      -> EF Core migration files
frontend/
  index.html       -> Login / Dashboard / Course detail views
  app.js           -> Fetch-based API calls and UI state
```

## API Endpoints
| Method | Route                          | Auth | Description                          |
|--------|---------------------------------|------|--------------------------------------|
| POST   | `/api/auth/login`               | No   | Authenticate, returns a JWT          |
| GET    | `/api/courses`                  | Yes  | Dashboard: all courses + progress    |
| GET    | `/api/courses/{id}`             | Yes  | Course detail + ordered lessons      |
| POST   | `/api/courses/enroll`           | Yes  | Enroll in a course                   |
| POST   | `/api/courses/lessons/complete` | Yes  | Mark a lesson as complete            |
