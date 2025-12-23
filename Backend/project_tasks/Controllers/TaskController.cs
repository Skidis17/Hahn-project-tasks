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

        // Get current user ID from JWT token
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            if (!long.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        // Get all tasks for a project
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

        // Get a specific task by ID
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

        // Create a new task
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

        // Update a task
        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask(long projectId, long taskId, [FromBody] UpdateTaskRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Invalid input", errors = ModelState });
                }

                var userId = GetCurrentUserId();
                var task = await _taskService.UpdateTaskAsync(taskId, projectId, request, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                return Ok(task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(500, new { message = "An error occurred while updating the task" });
            }
        }

        // Mark a task as completed
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

        // Delete a task
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
