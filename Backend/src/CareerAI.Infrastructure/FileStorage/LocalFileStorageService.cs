using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CareerAI.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorageService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(
        IFormFile file)
    {
        if (file.Length == 0)
        {
            throw new Exception("File is empty.");
        }

        var extension =
            Path.GetExtension(file.FileName)
                .ToLowerInvariant();

        var allowedExtensions = new[]
        {
            ".pdf",
            ".docx"
        };

        if (!allowedExtensions.Contains(extension))
        {
            throw new Exception(
                "Only PDF and DOCX files are allowed.");
        }

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "resumes");

        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName =
            $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(
            uploadsFolder,
            uniqueFileName);

        await using var stream =
            new FileStream(
                filePath,
                FileMode.Create);

        await file.CopyToAsync(stream);

        return Path.Combine(
            "resumes",
            uniqueFileName);
    }

    public Task DeleteFileAsync(
        string filePath)
    {
        var fullPath = Path.Combine(
            _environment.WebRootPath,
            filePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}