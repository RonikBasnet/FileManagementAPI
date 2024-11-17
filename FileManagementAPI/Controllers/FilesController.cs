//using FileManagementAPI.Models;
//using FileManagementAPI.Services;
//using Microsoft.AspNetCore.Mvc;
//using System.IO;

//namespace FileManagementAPI.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class FilesController : ControllerBase
//{
//    private readonly FileService _fileService;
//    private readonly IWebHostEnvironment _environment;

//    public FilesController(FileService fileService, IWebHostEnvironment environment)
//    {
//        _fileService = fileService;
//        _environment = environment;
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAllFiles()
//    {
//        var files = await _fileService.GetAllFilesAsync();
//        return Ok(files);
//    }

//    [HttpGet("{id}")]
//    public async Task<IActionResult> GetFileById(int id)
//    {
//        var file = await _fileService.GetFileByIdAsync(id);
//        if (file == null) return NotFound();
//        return Ok(file);
//    }

//    [HttpPost]
//    public async Task<IActionResult> CreateFile([FromForm] IFormFile file, [FromForm] string title, [FromForm] string category)
//    {
//        if (file == null || file.Length == 0)
//            return BadRequest("No file uploaded.");

//        try
//        {
//            // Ensure the upload directory exists.
//            string uploadDir = Path.Combine(_environment.WebRootPath, "uploads");
//            if (!Directory.Exists(uploadDir))
//                Directory.CreateDirectory(uploadDir);

//            // Generate a unique file name.
//            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
//            string filePath = Path.Combine(uploadDir, fileName);

//            // Save the file to the server.
//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            // Generate the file's public URL.
//            string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

//            // Create a FileModel instance.
//            var fileModel = new FileModel
//            {
//                Title = title,
//                Category = category,
//                FileUrl = fileUrl,
//                LastUpdated = DateTime.UtcNow,
//                Created = DateTime.UtcNow,
//                IsAssigned = false
//            };

//            // Insert file metadata into the database.
//            var newId = await _fileService.InsertFileAsync(fileModel);

//            return CreatedAtAction(nameof(GetFileById), new { id = newId }, fileModel);
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, $"Internal server error: {ex.Message}");
//        }
//    }

//    [HttpPut("{id}")]
//    public async Task<IActionResult> UpdateFile(int id, FileModel file)
//    {
//        if (id != file.Id) return BadRequest();
//        await _fileService.UpdateFileAsync(file);
//        return NoContent();
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteFile(int id)
//    {
//        await _fileService.DeleteFileAsync(id);
//        return NoContent();
//    }
//}
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

    //[HttpPost("upload")]
    //public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string title, [FromForm] string category)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("No file was uploaded.");

    //    try
    //    {
    //        // Ensure the uploads directory exists
    //        string uploadsDir = Path.Combine(_environment.WebRootPath, "uploads");
    //        if (!Directory.Exists(uploadsDir))
    //            Directory.CreateDirectory(uploadsDir);

    //        // Generate a unique file name
    //        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    //        string filePath = Path.Combine(uploadsDir, fileName);

    //        // Save the file to the server
    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        // Generate the file's public URL
    //        string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

    //        // Create a FileModel instance
    //        var fileModel = new FileModel
    //        {
    //            Title = title,
    //            Category = category,
    //            FileUrl = fileUrl,
    //            LastUpdated = DateTime.UtcNow,
    //            Created = DateTime.UtcNow,
    //            IsAssigned = false
    //        };

    //        // Insert file metadata into the database
    //        var newId = await _fileService.InsertFileAsync(fileModel);

    //        return CreatedAtAction(nameof(GetFileById), new { id = newId }, fileModel);
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Internal server error: {ex.Message}");
    //    }
    //}
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

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate the file's public URL
            //string fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            string fileUrl = $"/uploads/{fileName}";
            // Save file metadata including FileType
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(int id, FileModel file)
    {
        if (id != file.Id) return BadRequest();
        await _fileService.UpdateFileAsync(file);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        await _fileService.DeleteFileAsync(id);
        return NoContent();
    }
}
