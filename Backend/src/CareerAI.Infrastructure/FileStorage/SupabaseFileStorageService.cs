using Amazon.S3;
using Amazon.S3.Model;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CareerAI.Infrastructure.FileStorage;

public class SupabaseFileStorageService : IFileStorageService
{
    private const string BucketName = "resumes";

    private readonly IAmazonS3 _s3Client;

    public SupabaseFileStorageService(
        IConfiguration configuration)
    {
        var endpoint =
            configuration["Supabase:S3Endpoint"]
            ?? throw new InvalidOperationException(
                "Supabase S3 endpoint is missing.");

        var accessKey =
            configuration["Supabase:S3AccessKey"]
            ?? throw new InvalidOperationException(
                "Supabase S3 access key is missing.");

        var secretKey =
            configuration["Supabase:S3SecretKey"]
            ?? throw new InvalidOperationException(
                "Supabase S3 secret key is missing.");

        var region =
            configuration["Supabase:S3Region"]
            ?? throw new InvalidOperationException(
                "Supabase S3 region is missing.");

        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            AuthenticationRegion = region,
            ForcePathStyle = true
        };

        _s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            config);
    }

    public async Task<string> SaveFileAsync(IFormFile file)
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

        var uniqueFileName =
            $"{Guid.NewGuid()}{extension}";

        var objectKey = uniqueFileName;

        await using var stream =
            file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType =
          file.ContentType
          ?? "application/octet-stream",

            DisableDefaultChecksumValidation = true,
            DisablePayloadSigning = true
        };

        try
        {
            await _s3Client.PutObjectAsync(request);
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception(
                $"Supabase S3 upload failed. " +
                $"Status: {ex.StatusCode}. " +
                $"Details: {ex.Message}",
                ex);
        }

        return Path.Combine(
                BucketName,
                uniqueFileName)
            .Replace("\\", "/");
    }

    public async Task DeleteFileAsync(
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return;
        }

        var objectKey = filePath
            .Replace("\\", "/")
            .Replace(
                $"{BucketName}/",
                "",
                StringComparison.OrdinalIgnoreCase);

        var request = new DeleteObjectRequest
        {
            BucketName = BucketName,
            Key = objectKey
        };

        try
        {
            await _s3Client.DeleteObjectAsync(request);
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception(
                $"Supabase S3 delete failed. " +
                $"Status: {ex.StatusCode}. " +
                $"Details: {ex.Message}",
                ex);
        }
    }
}