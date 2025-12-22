# Hahn-project-tasks

Full-stack Project/Task manager with:

- **Backend**: ASP.NET Core Web API + EF Core + MySQL + JWT auth
- **Frontend**: React (Vite) + Axios + shadcn/ui

## Core Implemented Functionalities

### Authentication

- **Login** against the backend and store JWT in the browser.
- Authenticated routes are protected in the frontend.

### Projects (CRUD)

- Create project
- List projects (per authenticated user)
- View project details
- Update project
- Delete project

### Tasks (CRUD) inside a Project

- Create task (with due date)
- List tasks for a project
- Update task (title/description/status/due date)
- Mark task as completed
- Delete task

### Project Progress (Stats)

Backend computes and returns:

- **Total tasks**
- **Completed tasks**
- **Progress percentage** (0–100)

Progress is derived from task statuses (`Completed / Total * 100`). No DB column is required.

## Project Structure

```
Hahn-project-tasks/
  Backend/project_tasks/       # .NET API
  Frontend/                   # React app
```

## Prerequisites

- **Node.js** (LTS recommended)
- **.NET SDK** (7/8 is fine)
- **MySQL** (local instance)

## Ports

- **Backend API**: `http://localhost:5000`
- **Frontend** (Vite): typically `http://localhost:5173`

## Backend Setup (ASP.NET Core API)

### 1) Configure MySQL connection

The backend currently uses a connection string in:

`Backend/project_tasks/Program.cs`

Example (current style):

```text
server=localhost;port=3306;database=project_tasks;user=root;password=;AllowUserVariables=True;UseAffectedRows=False
```

Make sure:

- MySQL is running
- The user/password are correct

### 2) Run the backend

From `Backend/project_tasks/`:

```bash
dotnet restore
dotnet run
```

The API will start at:

- `http://localhost:5000`

The backend calls `EnsureCreated()` and seeds initial data via `DbInitializer.Initialize(db)`.

### 3) Swagger

In development you can open:

- `http://localhost:5000/swagger`

## Frontend Setup (React + Vite)

From `Frontend/`:

```bash
npm install
npm run dev
```

Then open the Vite URL shown in the terminal (typically `http://localhost:5173`).

## Authentication Notes

- The JWT token is stored in `localStorage` under: `JWT_Token`.
- Axios attaches the token as `Authorization: Bearer <token>`.

## API Overview

Base URL:

- `http://localhost:5000/api`

### Auth

- `POST /api/auth/login`

### Projects

- `GET /api/projects`
- `GET /api/projects/{id}`
- `POST /api/projects`
- `PUT /api/projects/{id}`
- `DELETE /api/projects/{id}`

### Project Progress

- `GET /api/projects/{id}/progress`

Returns:

```json
{
  "projectId": 1,
  "projectTitle": "My Project",
  "totalTasks": 5,
  "completedTasks": 2,
  "progressPercentage": 40
}
```

### Tasks (nested under Projects)

- `GET /api/projects/{projectId}/tasks`
- `GET /api/projects/{projectId}/tasks/{taskId}`
- `POST /api/projects/{projectId}/tasks`
- `PUT /api/projects/{projectId}/tasks/{taskId}`
- `PATCH /api/projects/{projectId}/tasks/{taskId}/complete`
- `DELETE /api/projects/{projectId}/tasks/{taskId}`

## Troubleshooting

### 405 Method Not Allowed on PUT/PATCH/DELETE

If you see a browser error like:

```
PUT http://localhost:5000/api/projects/1/tasks/1 405 (Method Not Allowed)
```

It can be caused by CORS preflight (`OPTIONS`) not being handled. Make sure you:

- Restarted the backend after changes
- Backend pipeline includes routing + CORS in the correct order (routing, then CORS, then auth)

### 401 Unauthorized

- Verify you are logged in
- Verify `JWT_Token` exists in localStorage
- Verify requests include `Authorization: Bearer ...`

## What is not stored in DB?

`totalTasks`, `completedTasks`, and `progressPercentage` are **computed values** returned in API responses.

They are not stored in DB because:

- They can be derived from tasks at any time
- Storing them risks the values getting out of sync

