using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This file contains the Project model representing the Project entity in the database

namespace Project_tasks.Models
{
    [Table("Project")]
    public class Project
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
        [Column("id_user")]
        public long IdUser { get; set; }

        [ForeignKey("IdUser")]
        public virtual User User { get; set; } = null!;
       
        // foreign key relationship
        public virtual ICollection<TodoItem> Tasks { get; set; } = new List<TodoItem>();
    }
}