using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Helpers
{
    public static class SeedData
    {
        public static async Task Initialize(LibraryContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Seed admin user
            var adminEmail = "admin@library.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    Address = "123 Admin Street",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(admin, new[] { "Admin", "User" });
                }
            }

            // Seed categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Fiction", Description = "Fictional books" },
                    new Category { Name = "Non-Fiction", Description = "Non-fictional books" },
                    new Category { Name = "Science", Description = "Science books" },
                    new Category { Name = "History", Description = "History books" },
                    new Category { Name = "Biography", Description = "Biography books" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed books
            if (!await context.Books.AnyAsync())
            {
                var fictionCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Fiction");
                var scienceCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Science");

                var books = new List<Book>
                {
                    new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "9780743273565",
                        Description = "A story of wealth, love, and the American Dream",
                        TotalCopies = 10,
                        AvailableCopies = 10,
                        PublishedDate = new DateTime(1925, 4, 10),
                        CategoryId = fictionCategory.Id
                    },
                    new Book
                    {
                        Title = "A Brief History of Time",
                        Author = "Stephen Hawking",
                        ISBN = "9780553380163",
                        Description = "A popular science book about cosmology",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        PublishedDate = new DateTime(1988, 1, 1),
                        CategoryId = scienceCategory.Id
                    }
                };

                await context.Books.AddRangeAsync(books);
                await context.SaveChangesAsync();
            }
        }
    }
}