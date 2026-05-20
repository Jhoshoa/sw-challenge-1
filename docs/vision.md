# Personal Task List Vision

## Product Summary

Personal Task List is a minimal REST API that allows a user to manage a simple personal task list.

The API supports creating tasks, viewing the task list, editing task details, marking tasks as completed, and deleting tasks. The implementation must follow the written specification exactly and avoid features that are not explicitly described.

## Challenge Goal

The main goal of this challenge is to practice **Spec-Driven Development (SDD)**.

The expected workflow is:

1. Define the product behavior in documentation.
2. Define the data model.
3. Define the API contract in OpenAPI.
4. Implement the API according to the contract.
5. Add tests that prove the implementation follows the specification.

The code is secondary to the discipline of writing the specification first and then following it strictly.

## Target User

The target user is a person who needs a simple way to track personal tasks through an API.

There is no multi-user behavior in scope. Authentication, authorization, teams, projects, categories, reminders, due dates, and task sharing are not part of this version.

## Core Capabilities

The API must allow the user to:

- View all tasks in the personal task list.
- Create a task with a required title and optional description.
- Edit an existing task's title and description.
- Mark an existing task as completed.
- Delete an existing task.

## Proposed API Scope

The first version should expose exactly five endpoints:

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `GET` | `/api/tasks` | Return the full task list. |
| `POST` | `/api/tasks` | Create a new task. |
| `PUT` | `/api/tasks/{id}` | Edit an existing task. |
| `PATCH` | `/api/tasks/{id}/complete` | Mark an existing task as completed. |
| `DELETE` | `/api/tasks/{id}` | Delete an existing task. |

`GET /api/tasks/{id}` is intentionally excluded from version 1 because the challenge does not require a separate task detail view. If that behavior is added later, it must first be added to the user stories and OpenAPI contract.

## Data Scope

The system needs one main entity: `Task`.

Recommended fields:

| Field | Purpose |
| --- | --- |
| `id` | Unique task identifier. |
| `title` | Required short task text. |
| `description` | Optional task details. |
| `isCompleted` | Indicates whether the task is completed. |
| `createdAt` | Timestamp when the task was created. |
| `updatedAt` | Timestamp when the task was last changed. |
| `completedAt` | Timestamp when the task was completed, if applicable. |

## Technical Direction

The implementation should use:

- ASP.NET Core Web API
- Entity Framework Core
- SQLite
- EF Core migrations
- Unit or integration tests for the specified behavior

SQLite is acceptable because it provides a real relational database workflow while keeping local setup lightweight.

## Success Criteria

The challenge is successful when:

- All required specification documents exist under `/docs`.
- The OpenAPI contract describes the exact API behavior.
- The C# Web API implements only the documented endpoints.
- SQLite persistence works through Entity Framework Core.
- EF Core migrations can create the database schema.
- Tests verify the core user stories and important error cases.
- No authentication or extra product features are added.

## Out Of Scope

The following are not part of this challenge:

- Authentication or authorization
- User accounts
- Multiple task lists
- Task categories, tags, priorities, due dates, or reminders
- Search, filtering, sorting, or pagination
- Frontend UI
- Background jobs
- Notifications
- Soft delete
- Audit history

These features should not be implemented unless the specification is intentionally changed first.
