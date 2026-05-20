# Personal Task List API

Minimal ASP.NET Core REST API for managing a personal task list. The project follows a spec-driven workflow: product scope, data model, and OpenAPI contract are documented first, and the implementation is kept aligned with that contract.

## Tech Stack

- ASP.NET Core 8
- Entity Framework Core
- SQLite
- FluentValidation
- xUnit integration tests
- OpenAPI contract in `docs/openapi.yaml`

## Project Structure

```text
docs/
  openapi.yaml              API contract
  vision.md                 Product scope and constraints
  data-model.md             Task entity model
  user-stories.md           User stories and acceptance criteria
src/
  PersonalTaskList.Api/     Web API implementation
    Domain/                 Task aggregate and repository port
    Application/            Task service and application DTOs
    Infrastructure/         EF Core persistence adapter, DbContext, and migrations
    Presentation/           MVC controllers, HTTP contracts, and validators
tests/
  PersonalTaskList.Api.Tests/
```

## API Scope

The API exposes exactly the documented task endpoints:

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `GET` | `/api/tasks` | Return all tasks. |
| `POST` | `/api/tasks` | Create a task. |
| `PUT` | `/api/tasks/{id}` | Edit a task. |
| `PATCH` | `/api/tasks/{id}/complete` | Mark a task as completed. |
| `DELETE` | `/api/tasks/{id}` | Delete a task. |

`GET /api/tasks/{id}` and other extra endpoints are intentionally out of scope for this version.

## Prerequisites

- .NET 8 SDK

## Run Locally

Restore dependencies:

```powershell
dotnet restore PersonalTaskList.sln
```

Apply the SQLite migration:

```powershell
dotnet ef database update --project src/PersonalTaskList.Api
```

Start the API:

```powershell
dotnet run --project src/PersonalTaskList.Api
```

The local API is configured for:

```text
http://localhost:5269
```

## Test

Run the full test suite:

```powershell
dotnet test PersonalTaskList.sln
```

The tests cover the documented endpoints, validation failures, missing task behavior, SQLite persistence, and contract alignment with `docs/openapi.yaml`.

## Example Requests

Create a task:

```http
POST http://localhost:5269/api/tasks
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs, and bread"
}
```

List tasks:

```http
GET http://localhost:5269/api/tasks
Accept: application/json
```

Mark a task as completed:

```http
PATCH http://localhost:5269/api/tasks/{id}/complete
Accept: application/json
```

## Documentation

- Product vision: `docs/vision.md`
- User stories: `docs/user-stories.md`
- Data model: `docs/data-model.md`
- OpenAPI contract: `docs/openapi.yaml`
