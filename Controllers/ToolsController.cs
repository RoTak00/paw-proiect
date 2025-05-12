using Microsoft.AspNetCore.Mvc;

namespace PAW_Project.Controllers;

public class ToolsController : Controller
{
    // Receive an image and a tool to use on the image through AJAX
    // Run the python script directly and present the saved image to the user
    [HttpPost]
    public async Task<IActionResult> ProcessImage(IFormFile imageFile, int toolId)
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("No file uploaded.");

        // You can now load tool from DB by toolId if needed
        // var tool = await _context.Tools.FindAsync(toolId);

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        // Simulate tool processing delay
        await Task.Delay(3000);

        var resultUrl = Url.Content($"~/uploads/{fileName}");

        return Json(new { success = true, resultUrl });
    }
}
