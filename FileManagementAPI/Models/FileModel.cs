namespace FileManagementAPI.Models;

public class FileModel
{
    public int Id { get; set; }
    public string FileType { get; set; }
    public string Title { get; set; }
    public string Category { get; set; }
    public string FileUrl { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime Created { get; set; }
    public bool IsAssigned { get; set; }
}
