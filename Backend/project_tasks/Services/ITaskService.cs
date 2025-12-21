using Project_tasks.DTO;

// This interface defines task-related services

namespace Project_tasks.Services
{
    public interface ITaskService
    {
        Task<List<TaskResponse>> GetAllTasksByProjectAsync(long projectId, long userId);
        Task<TaskResponse?> GetTaskByIdAsync(long taskId, long projectId, long userId);
        Task<TaskResponse?> CreateTaskAsync(CreateTaskRequest request, long projectId, long userId);
        Task<TaskResponse?> CompleteTaskAsync(long taskId, long projectId, long userId);
        Task<bool> DeleteTaskAsync(long taskId, long projectId, long userId);
    }
}