using System.Diagnostics;
using System.Security.Claims;
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
        else
        {
            var tokens = HttpContext.Session.GetString("UploadedTokens");
            var tokenList = string.IsNullOrEmpty(tokens)
                ? new List<string>()
                : tokens.Split(',').ToList();

            tokenList.Add(uploadFile.Token.ToString());
            HttpContext.Session.SetString("UploadedTokens", string.Join(",", tokenList.Distinct()));
        }
        
        
        Console.WriteLine(uploadFile);
        
        _context.UploadFiles.Add(uploadFile);
        await _context.SaveChangesAsync();
        
        
        
        return Json(new { success = true, message = "File saved successfully!", fileId = uploadFile.Token });

    }
    
    // Receive an image and a tool to use on the image through AJAX
    // Run the python script directly and present the saved image to the user
    [HttpPost]
    public async Task<IActionResult> ProcessImage(Guid file, int toolId)
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
            var outputFile = Path.GetFileNameWithoutExtension(uploadFile.FileName) + ".png";
            var outputPath = Path.Combine("/", foldername, outputFile);
            //Console.WriteLine(outputPath);
            var resultUrl = Url.Content(outputPath);
            var downloadName = Path.GetFileNameWithoutExtension(uploadFile.OriginalFileName) + "_" + Path.GetFileNameWithoutExtension(tool.ScriptPath) + ".png";
            
            //add the task to the database
            //Console.WriteLine(uploadFile);
            
            //Console.WriteLine(uploadFile.Id);

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
