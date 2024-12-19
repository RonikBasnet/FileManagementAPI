using FileManagementAPI.Models;
using FileManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace FileManagementAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly IWebHostEnvironment _environment;

    public FilesController(FileService fileService, IWebHostEnvironment environment)
    {
        _fileService = fileService;
        _environment = environment;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllFiles()
    {
        var files = await _fileService.GetAllFilesAsync();
        return Ok(files);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileById(int id)
    {
        var file = await _fileService.GetFileByIdAsync(id);
        if (file == null) return NotFound();
        return Ok(file);
    }

  
    [HttpPost("upload")]
    [RequestSizeLimit(10_000_000)] // Set maximum file size (10 MB in this case)
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string title, [FromForm] string category)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        try
        {
            // Ensure uploads directory exists
            string uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            // Generate unique filename
            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            string fileUrl = $"/uploads/{fileName}";
  
            var fileModel = new FileModel
            {
                Title = title,
                Category = category,
                FileUrl = fileUrl,
                FileType = file.ContentType,
                LastUpdated = DateTime.UtcNow,
                Created = DateTime.UtcNow,
                IsAssigned = false
            };

            var newId = await _fileService.InsertFileAsync(fileModel);
            return CreatedAtAction(nameof(GetFileById), new { id = newId }, fileModel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    //[HttpPut("{id}")]
    //public async Task<IActionResult> UpdateFile(int id, FileModel file)
    //{
    //    if (id != file.Id) return BadRequest();
    //    await _fileService.UpdateFileAsync(file);
    //    return NoContent();
    //}
    [HttpPut("{id}")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> UpdateFile(int id, [FromForm] IFormFile file, [FromForm] string title, [FromForm] string category)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file was uploaded.");

        try
        {
            var existingFile = await _fileService.GetFileByIdAsync(id);
            if (existingFile == null)
                return NotFound($"File with ID {id} not found.");

            string uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsDir, fileName);
            var oldFilePath = Path.Combine(uploadsDir, Path.GetFileName(existingFile.FileUrl.TrimStart('/')));
            if (System.IO.File.Exists(oldFilePath)) 
            {
                System.IO.File.Delete(oldFilePath); 
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Update the file's metadata
            existingFile.Title = title;
            existingFile.Category = category;
            existingFile.FileType = file.ContentType;
            existingFile.LastUpdated = DateTime.UtcNow;
            existingFile.FileUrl = $"/uploads/{fileName}";
            await _fileService.UpdateFileAsync(existingFile);

            return Ok(existingFile);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        await _fileService.DeleteFileAsync(id);
        return NoContent();
    }
}
