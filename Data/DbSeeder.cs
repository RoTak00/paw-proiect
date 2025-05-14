using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    
    public static async Task SeedImageToolsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var toolsToSeed = new List<ImageTool>
        {
            new ImageTool { Name = "Background Removal", Description = "Automatically removes the background from an image, leaving only the main subject. Useful for product photos or profile pictures.", ScriptPath = "PythonScripts/background_removal.py" },
            new ImageTool { Name = "Color Space Transformation", Description = "Converts an image from one color space (e.g., RGB) to another (e.g., grayscale or HSV), allowing for advanced color-based processing.", ScriptPath = "PythonScripts/color_space_transformation.py" },
            new ImageTool { Name = "Contour Detection", Description = "Identifies the outlines of objects within an image, helpful for shape analysis, object segmentation, or feature extraction.", ScriptPath = "PythonScripts/contour_detection.py" },
            new ImageTool { Name = "Face Detection", Description = "Detects human faces within an image using machine learning models. Commonly used in surveillance, photo tagging, and biometrics.", ScriptPath = "PythonScripts/face_detection.py" },
            new ImageTool { Name = "Object Detection", Description = "Identifies and locates multiple objects (e.g., cars, people) within an image. Useful in robotics, automation, and real-time monitoring.", ScriptPath = "PythonScripts/object_detection.py" },
            new ImageTool { Name = "Resize", Description = "Changes the width and height of an image to a new resolution. Helps with standardizing image sizes or preparing inputs for AI models.", ScriptPath = "PythonScripts/resize.py" },
            new ImageTool { Name = "Text Recognition", Description = "Extracts text from an image using Optical Character Recognition (OCR). Useful for digitizing documents, signs, or license plates.", ScriptPath = "PythonScripts/text_recognition.py" }
        };

        foreach (var tool in toolsToSeed)
        {
            var exists = await context.ImageTools.AnyAsync(t => t.Name == tool.Name);
            if (!exists)
            {
                context.ImageTools.Add(tool);
            }
        }

        await context.SaveChangesAsync();
    }


}