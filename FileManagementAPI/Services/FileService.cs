using Dapper;
using FileManagementAPI.DataAccess;
using FileManagementAPI.Models;

namespace FileManagementAPI.Services;

public class FileService
{
    private readonly DapperContext _context;

    public FileService(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FileModel>> GetAllFilesAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<FileModel>("usp_GetAllFiles", commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<FileModel?> GetFileByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<FileModel>(
            "usp_GetFileById",
            new { Id = id },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<int> InsertFileAsync(FileModel file)
    {
        using var connection = _context.CreateConnection();
        var parameters = new
        {
            file.FileType,
            file.Title,
            file.Category,
            file.FileUrl,
            file.Created,
            file.LastUpdated,
            file.IsAssigned
        };
        return await connection.QuerySingleAsync<int>(
            "usp_InsertFile",
            parameters,
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<int> UpdateFileAsync(FileModel file)
    {
        using var connection = _context.CreateConnection();
        var parameters = new
        {
            file.Id,
            file.FileType,
            file.Title,
            file.Category,
            file.FileUrl,
            file.IsAssigned
        };
        return await connection.ExecuteAsync("usp_UpdateFile", parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> DeleteFileAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync("usp_DeleteFile", new { Id = id }, commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<int> UpdateFileAssignmentAsync(int id, bool isAssigned)
    {
        using var connection = _context.CreateConnection();
        var parameters = new { Id = id, IsAssigned = isAssigned };
        return await connection.ExecuteAsync("usp_UpdateFileAssignment", parameters, commandType: System.Data.CommandType.StoredProcedure);
    }

}
