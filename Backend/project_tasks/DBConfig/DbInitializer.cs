using Project_tasks.Models;
using Project_tasks.Services; 

// This file contains the database initializer to seed initial data

namespace Project_tasks.DBConfig
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if users already exist
            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Create test users with hashed passwords
            var users = new User[]
            {
                new User
                {
                    Email = "test1@hahn.com",
                    // Password: "test123" - hashed with BCrypt for security
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("test123")
                },
                new User
                {
                    Email = "test2@hahn.com",
                    // Password: "test1234" - hashed with BCrypt for security
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("test1234")
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}