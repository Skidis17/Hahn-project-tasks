using Project_tasks.DTO;

// This interface defines project-related services
// We used interfaces mainly for better abstraction and easier unit testing

namespace Project_tasks.Services
{
    public interface IProjectService
    {
        Task<List<ProjectResponse>> GetAllProjectsByUserAsync(long userId);
        Task<ProjectResponse?> GetProjectByIdAsync(long projectId, long userId);
        Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, long userId);
        Task<bool> DeleteProjectAsync(long projectId, long userId);
        Task<ProjectProgressResponse?> GetProjectProgressAsync(long projectId, long userId);
    }
}