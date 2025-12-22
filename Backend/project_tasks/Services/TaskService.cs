using Microsoft.EntityFrameworkCore;
using Project_tasks.DBConfig;
using Project_tasks.DTO;
using Project_tasks.Models;

// This interface defines project-related services

namespace Project_tasks.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskResponse>> GetAllTasksByProjectAsync(long projectId, long userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            if (project == null)
            {
                return new List<TaskResponse>();
            }

            return project.Tasks.Select(todo => MapToTaskResponse(todo)).ToList();
        }

        public async Task<TaskResponse?> GetTaskByIdAsync(long taskId, long projectId, long userId)
        {
            var todo = await _context.Tasks
                .Include(todo => todo.Project)
                .FirstOrDefaultAsync(todo => todo.Id == taskId && todo.IdProject == projectId && todo.Project.IdUser == userId);

            return todo == null ? null : MapToTaskResponse(todo);
        }

        public async Task<TaskResponse?> CreateTaskAsync(CreateTaskRequest request, long projectId, long userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            if (project == null)
            {
                return null;
            }

            var todo = new TodoItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                Status = TodoStatus.Pending,
                IdProject = projectId
            };

            _context.Tasks.Add(todo);
            await _context.SaveChangesAsync();

            return MapToTaskResponse(todo);
        }

        public async Task<TaskResponse?> UpdateTaskAsync(long taskId, long projectId, UpdateTaskRequest request, long userId)
        {
            var todo = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.IdProject == projectId && t.Project.IdUser == userId);

            if (todo == null)
            {
                return null;
            }

            todo.Title = request.Title;
            todo.Description = request.Description;
            todo.DueDate = request.DueDate;

            if (!Enum.TryParse<TodoStatus>(request.Status, ignoreCase: true, out var parsedStatus))
            {
                throw new ArgumentException("Invalid status value");
            }

            todo.Status = parsedStatus;

            await _context.SaveChangesAsync();

            return MapToTaskResponse(todo);
        }

        public async Task<TaskResponse?> CompleteTaskAsync(long taskId, long projectId, long userId)
        {
            var todo = await _context.Tasks
                .Include(todo => todo.Project)
                .FirstOrDefaultAsync(todo => todo.Id == taskId && todo.IdProject == projectId && todo.Project.IdUser == userId);

            if (todo == null)
            {
                return null;
            }

            todo.Status = TodoStatus.Completed;
            await _context.SaveChangesAsync();

            return MapToTaskResponse(todo);
        }

        public async Task<bool> DeleteTaskAsync(long taskId, long projectId, long userId)
        {
            var todo = await _context.Tasks
                .Include(todo => todo.Project)
                .FirstOrDefaultAsync(todo => todo.Id == taskId && todo.IdProject == projectId && todo.Project.IdUser == userId);

            if (todo == null)
            {
                return false;
            }

            _context.Tasks.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        private TaskResponse MapToTaskResponse(TodoItem todo)
        {
            return new TaskResponse
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Status = todo.Status.ToString(),
                DueDate = todo.DueDate,
                ProjectId = todo.IdProject
            };
        }
    }
}