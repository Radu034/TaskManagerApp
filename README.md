# Task Manager Application

![CI/CD](https://github.com/Radu034/TaskManagerApp/actions/workflows/ci.yml/badge.svg)

## Descriere

REST API pentru managementul task-urilor in echipa. Backend C# / ASP.NET Core 8.

## Tehnologii

- C# / ASP.NET Core 8 Web API
- Entity Framework Core 8 + SQLite
- JWT Bearer Authentication
- xUnit + FluentAssertions (unit testing)
- Roslyn Analyzers (static code analysis, built-in .NET)
- GitHub Actions (CI/CD)
- Swagger / OpenAPI

## Structura proiect

```
TaskManagerApp/
├── src/
│   └── TaskManagerApp.API/
│       ├── Controllers/     # AuthController, TasksController
│       ├── Data/            # AppDbContext (EF Core)
│       ├── DTOs/            # Request/Response DTOs
│       ├── Migrations/      # EF Core migrations
│       ├── Models/          # AppUser, TaskItem
│       ├── Services/        # ITaskService, IAuthService + implementations
│       └── Program.cs
├── tests/
│   └── TaskManagerApp.Tests/
│       ├── TaskServiceTests.cs    # 9 unit tests
│       ├── AuthServiceTests.cs    # 3 unit tests
│       └── TaskItemModelTests.cs  # 2 unit tests
└── .github/
    └── workflows/
        └── ci.yml           # CI/CD pipeline
```

## Cum rulezi local

```bash
dotnet restore
dotnet ef database update --project src/TaskManagerApp.API
dotnet run --project src/TaskManagerApp.API
```

Swagger UI disponibil la: `https://localhost:5001/swagger`

## Autentificare

```bash
# Inregistrare
POST /api/auth/register
{ "email": "user@example.com", "password": "password123", "name": "John" }

# Login (returneaza JWT token)
POST /api/auth/login
{ "email": "user@example.com", "password": "password123" }

# Foloseste token-ul in header:
Authorization: Bearer <token>
```

## Endpoints Task-uri

| Metoda | URL | Descriere |
|--------|-----|-----------|
| GET | /api/tasks | Lista task-uri (filtrare: ?status=Todo&priority=High) |
| GET | /api/tasks/{id} | Un task specific |
| POST | /api/tasks | Creeaza task |
| PUT | /api/tasks/{id} | Actualizeaza task |
| DELETE | /api/tasks/{id} | Sterge task |

## Rulare teste

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Static Code Analysis

```bash
dotnet build /p:AnalysisMode=All /p:EnforceCodeStyleInBuild=true
```

## CI/CD Pipeline

Pipeline-ul GitHub Actions ruleaza automat la fiecare push:

1. **Build** — `dotnet build --configuration Release`
2. **Static Code Analysis** — Roslyn Analyzers (fara tool-uri externe), raport uploadat ca artifact
3. **Unit Tests** — `dotnet test` cu coverage report, rezultate uploadate ca artifact
4. **Deploy** (doar pe `main`) — `dotnet publish` + copiere artefacte, pachet disponibil 30 zile
