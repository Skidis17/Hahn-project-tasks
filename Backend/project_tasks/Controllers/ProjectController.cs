using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Project_tasks.DTO;
using Project_tasks.Services;

// This controller manages project-related endpoints

namespace Project_tasks.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Middleware 
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user ID from JWT token
        /// </summary>
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return long.Parse(userIdClaim);
        }

        /// <summary>
        /// Get all projects for the authenticated user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var userId = GetCurrentUserId();
                var projects = await _projectService.GetAllProjectsByUserAsync(userId);
                return Ok(projects);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, new { message = "An error occurred while fetching projects" });
            }
        }

        /// <summary>
        /// Get a specific project by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(long id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var project = await _projectService.GetProjectByIdAsync(id, userId);

                if (project == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                return Ok(project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project");
                return StatusCode(500, new { message = "An error occurred while fetching the project" });
            }
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input", errors = ModelState });
                }

                var userId = GetCurrentUserId();
                var project = await _projectService.CreateProjectAsync(request, userId);

                return CreatedAtAction(nameof(GetProjectById), new { id = project.Id }, project);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, new { message = "An error occurred while creating the project" });
            }
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(long id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _projectService.DeleteProjectAsync(id, userId);

                if (!success)
                {
                    return NotFound(new { message = "Project not found" });
                }

                return Ok(new { message = "Project deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project");
                return StatusCode(500, new { message = "An error occurred while deleting the project" });
            }
        }

        /// <summary>
        /// Get project progress (total tasks, completed tasks, percentage)
        /// </summary>
        [HttpGet("{id}/progress")]
        public async Task<IActionResult> GetProjectProgress(long id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var progress = await _projectService.GetProjectProgressAsync(id, userId);

                if (progress == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                return Ok(progress);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting project progress");
                return StatusCode(500, new { message = "An error occurred while fetching project progress" });
            }
        }
    }
}