using System.ComponentModel.DataAnnotations;

// This file contains DTOs related to Projects

namespace Project_tasks.DTO
{
    // CreateProjectRequest.cs
    public class CreateProjectRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Description cannot exceed 255 characters")]
        public string? Description { get; set; }
    }

    // ProjectResponse.cs
    public class ProjectResponse
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long UserId { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressPercentage { get; set; }
    }

    // ProjectProgressResponse.cs
    public class ProjectProgressResponse
    {
        public long ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public double ProgressPercentage { get; set; }
    }
}