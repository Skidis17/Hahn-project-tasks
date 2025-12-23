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

## How to Run

### MySQL
1. Start your MySQL server (via command line, MySQL Workbench, or system service)
2. Ensure MySQL is running on the default port `3306`
3. Create a database for the project or let EF Core migrations create it

### Backend
1. Navigate to the backend directory:
```bash
   cd Backend/project_tasks
```
2. Restore dependencies:
```bash
   dotnet restore
```
3. Apply database migrations:
```bash
   dotnet ef database update
```
4. Run the API:
```bash
   dotnet run
```
5. Backend will be available at `http://localhost:5000`

### Frontend
1. Navigate to the frontend directory:
```bash
   cd Frontend
```
2. Install dependencies:
```bash
   npm install
```
3. Start the development server:
```bash
   npm run dev
```
4. Frontend will be available at `http://localhost:5173`

## Demo video link

This is the drive link for the demo video: [**Bold Link**](https://drive.google.com/drive/folders/1Guw4sF0EknoIKxs3iiwGWddQTXTeUc1i?usp=sharing) 
