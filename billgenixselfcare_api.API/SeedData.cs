using billgenixselfcare_api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles
        string[] roles = { "Super Admin", "Admin", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create admin user
        var adminEmail = "admin@gmail.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Name = "Admin",
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");

                // Add claims
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "ManageUsers"));
                await userManager.AddClaimAsync(adminUser, new Claim("Permission", "ManageDepartments"));
            }
        }
    }
}
