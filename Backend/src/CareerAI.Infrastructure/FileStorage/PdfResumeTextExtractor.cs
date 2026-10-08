using Amazon.S3;
using CareerAI.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using UglyToad.PdfPig;

namespace CareerAI.Infrastructure.FileStorage;

public class PdfResumeTextExtractor : IResumeTextExtractor
{
    private const string BucketName = "resumes";

    private readonly IAmazonS3 _s3Client;

    public PdfResumeTextExtractor(
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

    public async Task<string> ExtractTextAsync(
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "Resume file path is empty.",
                nameof(filePath));
        }

        var objectKey = filePath
            .Replace("\\", "/")
            .Replace(
                $"{BucketName}/",
                "",
                StringComparison.OrdinalIgnoreCase);

        try
        {
            var response =
                await _s3Client.GetObjectAsync(
                    BucketName,
                    objectKey);

            await using var pdfStream =
                response.ResponseStream;

            using var document =
                PdfDocument.Open(pdfStream);

            var text = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }

            return text.ToString();
        }
        catch (AmazonS3Exception ex)
        {
            throw new FileNotFoundException(
                "Resume could not be downloaded from Supabase Storage.",
                ex);
        }
    }
}