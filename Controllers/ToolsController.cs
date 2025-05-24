using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAW_Project.Data;
using PAW_Project.Models;

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

    [HttpPost]
    public async Task<IActionResult> SaveUploadedFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return Json(new { success = false, message = "No file could be found..." });
        }
        
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }
        
        var originalFileName = Path.GetFileName(file.FileName);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
    
        // After saving the file to the server, 
        // Also save it to the database, initially as a temp file
        UploadFile uploadFile = new UploadFile
        {
            FileName = fileName,
            AddedDate = DateTime.Now,
            OriginalFileName = originalFileName,
            IsTemp = true,
            Token = Guid.NewGuid()
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                uploadFile.UserId = userId;
            }
        }
        
        
        Console.WriteLine(uploadFile);
        
        _context.UploadFiles.Add(uploadFile);
        await _context.SaveChangesAsync();
        
        
        
        return Json(new { success = true, message = "File saved successfully!", fileId = uploadFile.Token });

    }
    
    // Receive an image and a tool to use on the image through AJAX
    // Run the python script directly and present the saved image to the user
    [HttpPost]
    public async Task<IActionResult> ProcessImage(Guid file, int toolId, string? options)
    {
        
        var tool = await _context.ImageTools.FindAsync(toolId);
        if (tool == null)
        {
            return BadRequest("Tool not found.");
        }

        string? userId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        
        var uploadFile = await _context.UploadFiles.Where(f => f.Token == file && f.UserId == userId).FirstOrDefaultAsync();

        if (uploadFile == null)
        {
            return BadRequest("File not found.");
        }
        
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var filePath = Path.Combine(uploadsPath, uploadFile.FileName);

        if (!System.IO.File.Exists(filePath))
        {
            return BadRequest("File not found.");
        }

        // Extract the JSON options from the AJAX request
        var parsedOptions = (options != null) ? JsonSerializer.Deserialize<Dictionary<string, string>>(options) ?? new Dictionary<string, string>():
            new Dictionary<string, string>();
        
        String venvPath = Path.Combine(Directory.GetCurrentDirectory(), ".venv/bin/python");
        String scriptPath = Path.Combine(Directory.GetCurrentDirectory(), tool.ScriptPath);

        var arguments = $"\"{scriptPath}\" \"{filePath}\"";

        foreach (var toolOptions in parsedOptions)
        {
            arguments += $" {toolOptions.Value} ";
        }
        
        Console.WriteLine(arguments);
        
        
        var startInfo = new ProcessStartInfo
        {
            FileName = venvPath,
            Arguments = arguments,
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
            var outputFile = Path.GetFileNameWithoutExtension(uploadFile.FileName);
            switch (toolId)
            {
                case 2:
                    if (parsedOptions.TryGetValue("tool_input_color_space", out var colorSpace))
                    {
                        outputFile += $"_{colorSpace.Trim()}";
                    }
                    break;
                case 6:
                    if (parsedOptions.TryGetValue("tool_input_width", out var widthStr) &&
                        parsedOptions.TryGetValue("tool_input_height", out var heightStr))
                    {
                        var width = int.Parse(Regex.Match(widthStr, @"\d+").Value);
                        var height = int.Parse(Regex.Match(heightStr, @"\d+").Value);
                        outputFile += $"_{width}x{height}";
                    }
                    break;
            }
            
            outputFile += ".png";
            var outputPath = Path.Combine("/", foldername, outputFile);
            var resultUrl = Url.Content(outputPath);
            var downloadName = Path.GetFileNameWithoutExtension(uploadFile.OriginalFileName) + "_" + Path.GetFileNameWithoutExtension(tool.ScriptPath) + ".png";
            
            var outputFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", foldername, outputFile);

            if (!System.IO.File.Exists(outputFullPath))
            {
                _logger.LogError($"Output file was not found: {outputFullPath}");
                return BadRequest("The output file could not be found after processing.");
            }

            uploadFile.IsTemp = false;
            _context.ImageTasks.Add(new ImageTask()
            {
                FileId = uploadFile.Id,
                OutputPath = outputPath,
                ImageToolId = tool.Id
            });
            
            await _context.SaveChangesAsync();
        

            return Json(new { success = true, resultUrl = resultUrl, downloadName = downloadName });
        }
    }
}
