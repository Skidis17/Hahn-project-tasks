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


## Project Structure

```
Hahn-project-tasks/
  Backend/project_tasks/       # .NET API
  Frontend/                   # React app
```

## Prerequisites

- **Node.js** (LTS recommended)
- **.NET SDK** (10)
- **MySQL** (local instance)

## Ports

- **Backend API**: `http://localhost:5000`
- **Frontend** (Vite): typically `http://localhost:5173`

## Authentication Notes

- The JWT token is stored in `localStorage` under: `JWT_Token`.
- Axios attaches the token as `Authorization: Bearer <token>`.


