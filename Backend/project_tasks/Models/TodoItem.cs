using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This file contains the TodoItem model representing the Task entity in the database
// I renamed this class to TodoItem to avoid conflicts with C# reserved keyword "Task" 

namespace Project_tasks.Models
{
    public enum TodoStatus
    {
        Pending,
        InProgress,
        Completed
    }

    [Table("Task")] 
    public class TodoItem
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("title")]
        [MaxLength(255)]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        [Column("status")]
        public TodoStatus Status { get; set; } = TodoStatus.Pending;

        [Required]
        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Required]
        [Column("id_project")]
        public long IdProject { get; set; }

        // Another method to define foreign key relationship (We used Icollection in Project.cs)
        [ForeignKey("IdProject")]
        public virtual Project Project { get; set; } = null!;
    }
}