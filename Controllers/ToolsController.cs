using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PAW_Project.Data;
using PAW_Project.Models;
using ZstdSharp.Unsafe;

namespace PAW_Project.Controllers;

public class ToolsController : Controller
{
    private readonly ILogger<ToolsController> _logger;
    private readonly AppDbContext _context;

    public ToolsController(ILogger<ToolsController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    // Receive an image and a tool to use on the image through AJAX
    // Run the python script directly and present the saved image to the user
    [HttpPost]
    public async Task<IActionResult> ProcessImage(IFormFile imageFile, int toolId)
    {
        if (imageFile == null || imageFile.Length == 0)
            return BadRequest("No file uploaded.");

        // You can now load a tool from DB by toolId if needed
        // var tool = await _context.Tools.FindAsync(toolId);
        var tool = await _context.ImageTools.FindAsync(toolId);

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploadsPath);

        var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }
        if (tool == null)
        {
            return BadRequest("Tool not found.");
        }

        UploadFile uploadFile = null;
        // add the update file in database
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                uploadFile = new UploadFile
                {
                    FileName = fileName,
                    UserId = userId,
                    AddedDate = DateTime.Now
                };

                _context.UploadFiles.Add(uploadFile);
                await _context.SaveChangesAsync();
            }
        }
        
        // Simulate tool processing delay
        String venvPath = Path.Combine(Directory.GetCurrentDirectory(), ".venv/bin/python");
        String scriptPath = Path.Combine(Directory.GetCurrentDirectory(), tool.ScriptPath);
        
        var startInfo = new ProcessStartInfo
        {
            FileName = venvPath,
            Arguments = scriptPath + " " + filePath,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return BadRequest("Python error:" + error);
            }

            var foldername = "output_" + Path.GetFileNameWithoutExtension(tool.ScriptPath);
            var outputFile = Path.GetFileNameWithoutExtension(fileName) + ".png";
            var outputPath = Path.Combine("~/",foldername, outputFile);
            Console.WriteLine(outputPath);
            var resultUrl = Url.Content(outputPath);
            
            // add the task to the database
            Console.WriteLine(uploadFile);
            if (uploadFile != null)
            {
                Console.WriteLine(uploadFile.Id);
                _context.ImageTasks.Add(new ImageTask()
                {
                    FileId = uploadFile.Id,
                    OutputPath = outputPath,
                    ImageToolId = tool.Id
                });
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true, resultUrl });
        }
    }
}
