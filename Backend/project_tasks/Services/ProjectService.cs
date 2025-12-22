using Microsoft.EntityFrameworkCore;
using Project_tasks.DBConfig;
using Project_tasks.DTO;
using Project_tasks.Models;

// This class implements project-related services

namespace Project_tasks.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _context;

        public ProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectResponse>> GetAllProjectsByUserAsync(long userId)
        {
            var projects = await _context.Projects
                .Where(p => p.IdUser == userId)
                .Include(p => p.Tasks)
                .ToListAsync();

            return projects.Select(p => MapToProjectResponse(p)).ToList();
        }

        public async Task<ProjectResponse?> GetProjectByIdAsync(long projectId, long userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            return project == null ? null : MapToProjectResponse(project);
        }

        public async Task<ProjectResponse> CreateProjectAsync(CreateProjectRequest request, long userId)
        {
            var project = new Project
            {
                Title = request.Title,
                Description = request.Description,
                IdUser = userId
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return MapToProjectResponse(project);
        }

        public async Task<ProjectResponse?> UpdateProjectAsync(long projectId, UpdateProjectRequest request, long userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            if (project == null)
            {
                return null;
            }

            project.Title = request.Title;
            project.Description = request.Description;

            await _context.SaveChangesAsync();

            return MapToProjectResponse(project);
        }

        public async Task<bool> DeleteProjectAsync(long projectId, long userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            if (project == null)
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProjectProgressResponse?> GetProjectProgressAsync(long projectId, long userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.IdUser == userId);

            if (project == null)
            {
                return null;
            }

            var totalTasks = project.Tasks.Count;
            var completedTasks = project.Tasks.Count(todo => todo.Status == TodoStatus.Completed);
            var progressPercentage = totalTasks == 0 ? 0 : (double)completedTasks / totalTasks * 100;

            return new ProjectProgressResponse
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                ProgressPercentage = Math.Round(progressPercentage, 2)
            };
        }

        private ProjectResponse MapToProjectResponse(Project project)
        {
            var totalTasks = project.Tasks?.Count ?? 0;
            var completedTasks = project.Tasks?.Count(todo => todo.Status == TodoStatus.Completed) ?? 0;
            var progressPercentage = totalTasks == 0 ? 0 : (double)completedTasks / totalTasks * 100;

            return new ProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                UserId = project.IdUser,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                ProgressPercentage = Math.Round(progressPercentage, 2)
            };
        }
    }
}