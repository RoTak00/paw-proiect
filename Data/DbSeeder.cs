using Microsoft.AspNetCore.Identity;
using PAW_Project.Models;

namespace PAW_Project.Data;

public static class DbSeeder
{
    // If the Roles do not exist in the database, create them
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    
    // Create 2 test users, one admin and one non-admin for testing purposes
    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Admin user
        string adminUsername = "admin";
        string adminEmail = "admin@image.com";
        string adminPassword = "Adminpa55!";
        string adminRole = "Admin";

        var admin = await userManager.FindByNameAsync(adminUsername);
        if (admin == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }

        // Normal user
        string userUsername = "user";
        string userEmail = "user@image.com";
        string userPassword = "Userpa55!";
        string userRole = "User";

        var user = await userManager.FindByNameAsync(userUsername);
        if (user == null)
        {
            var normalUser = new ApplicationUser
            {
                UserName = userUsername,
                Email = userEmail
            };
            var result = await userManager.CreateAsync(normalUser, userPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, userRole);
            }
        }
    }

}