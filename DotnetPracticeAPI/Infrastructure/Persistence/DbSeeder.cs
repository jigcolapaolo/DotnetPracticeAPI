using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public static class DbSeeder
    {
        public static async Task Seed(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!context.Users.Any())
            {
                var adminRole = await roleManager.FindByNameAsync("Admin");
                if (adminRole == null)
                {
                    adminRole = new IdentityRole<Guid>("Admin");
                    await roleManager.CreateAsync(adminRole);
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    Email = "admin@example.com",
                    CreatedAt = DateTime.UtcNow,
                };

                await userManager.CreateAsync(user, "Hashedpassword123-");
                await userManager.AddToRoleAsync(user, "Admin");

                context.Products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product1",
                    Description = "Description of Product1",
                    Price = 16.5M,
                    UserId = user.Id
                });

                context.SaveChanges();
            }
        }
    }
}
