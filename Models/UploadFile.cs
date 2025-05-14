namespace PAW_Project.Models;

public class UploadFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string UserId { get; set; }
    public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    
    public ApplicationUser User { get; set; } = null!;
}