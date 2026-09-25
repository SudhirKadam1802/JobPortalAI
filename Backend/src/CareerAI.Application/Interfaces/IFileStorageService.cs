using Microsoft.AspNetCore.Http;

namespace CareerAI.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        IFormFile file);

    Task DeleteFileAsync(
        string filePath);
}