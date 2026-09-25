using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using UglyToad.PdfPig;

namespace CareerAI.Infrastructure.FileStorage;

public class PdfResumeTextExtractor : IResumeTextExtractor
{
    private readonly IWebHostEnvironment _environment;

    public PdfResumeTextExtractor(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> ExtractTextAsync(
        string filePath)
    {
        return await Task.Run(() =>
        {
            // Convert relative path:
            // resumes/filename.pdf
            //
            // into physical path:
            // wwwroot/resumes/filename.pdf

            var physicalPath = Path.Combine(
                _environment.WebRootPath,
                filePath);

            if (!File.Exists(physicalPath))
            {
                throw new FileNotFoundException(
                    "Resume file was not found.",
                    physicalPath);
            }

            using var document =
                PdfDocument.Open(physicalPath);

            var text = new StringBuilder();

            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }

            return text.ToString();
        });
    }
}