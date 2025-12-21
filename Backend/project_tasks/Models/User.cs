using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// This file contains the User model representing the User entity in the database

namespace Project_tasks.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [Column("email")]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("hashed_password")]
        [MaxLength(255)]
        public string HashedPassword { get; set; } = string.Empty;

        // foreign key relationship
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}