using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Project_tasks.DTO;
using Project_tasks.Services;

// This controller manages task-related endpoints within projects

namespace Project_tasks.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/[controller]")]
    [Authorize] // Middleware
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
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
        /// Get all tasks for a project
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllTasks(long projectId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var tasks = await _taskService.GetAllTasksByProjectAsync(projectId, userId);
                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, new { message = "An error occurred while fetching tasks" });
            }
        }

        /// <summary>
        /// Get a specific task by ID
        /// </summary>
        [HttpGet("{taskId}")]
        public async Task<IActionResult> GetTaskById(long projectId, long taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.GetTaskByIdAsync(taskId, projectId, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting task");
                return StatusCode(500, new { message = "An error occurred while fetching the task" });
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTask(long projectId, [FromBody] CreateTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input", errors = ModelState });
                }

                var userId = GetCurrentUserId();
                var task = await _taskService.CreateTaskAsync(request, projectId, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                return CreatedAtAction(nameof(GetTaskById), 
                    new { projectId = projectId, taskId = task.Id }, task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        /// <summary>
        /// Mark a task as completed
        /// </summary>
        [HttpPatch("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(long projectId, long taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var task = await _taskService.CompleteTaskAsync(taskId, projectId, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(task);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task");
                return StatusCode(500, new { message = "An error occurred while completing the task" });
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask(long projectId, long taskId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _taskService.DeleteTaskAsync(taskId, projectId, userId);

                if (!success)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(new { message = "Task deleted successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }
    }
}

// API Endpoints short recap:

// POST   /api/auth/login                              - Login (No Auth)
// GET    /api/auth/test                               - Test endpoint (No Auth)

// GET    /api/projects                                - Get all projects (Auth Required)
// GET    /api/projects/{id}                           - Get project by ID (Auth Required)
// POST   /api/projects                                - Create project (Auth Required)
// DELETE /api/projects/{id}                           - Delete project (Auth Required)
// GET    /api/projects/{id}/progress                  - Get project progress (Auth Required)

// GET    /api/projects/{projectId}/tasks              - Get all tasks (Auth Required)
// GET    /api/projects/{projectId}/tasks/{taskId}     - Get task by ID (Auth Required)
// POST   /api/projects/{projectId}/tasks              - Create task (Auth Required)
// PATCH  /api/projects/{projectId}/tasks/{taskId}/complete - Complete task (Auth Required)
// DELETE /api/projects/{projectId}/tasks/{taskId}     - Delete task (Auth Required)