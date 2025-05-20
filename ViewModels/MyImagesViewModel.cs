using PAW_Project.Models;

namespace PAW_Project.ViewModels;

public class MyImagesViewModel
{
    public List<UploadFile> UploadedFiles { get; set; } = new();
    public Dictionary<string, List<ImageTask>> TasksByTool { get; set; } = new();
}