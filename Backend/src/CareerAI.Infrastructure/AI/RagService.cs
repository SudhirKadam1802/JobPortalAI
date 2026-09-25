using System.Text;
using System.Text.Json;
using CareerAI.Application.Features.RAG.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;

namespace CareerAI.Infrastructure.AI;

public class RagService : IRagService
{
    private readonly IRagRepository _ragRepository;
    private readonly IResumeRepository _resumeRepository;
    private readonly IResumeTextExtractor _textExtractor;
    private readonly IEmbeddingService _embeddingService;
    private readonly IOllamaService _ollamaService;

    public RagService(
        IRagRepository ragRepository,
        IResumeRepository resumeRepository,
        IResumeTextExtractor textExtractor,
        IEmbeddingService embeddingService,
        IOllamaService ollamaService)
    {
        _ragRepository = ragRepository;
        _resumeRepository = resumeRepository;
        _textExtractor = textExtractor;
        _embeddingService = embeddingService;
        _ollamaService = ollamaService;
    }

    public async Task<RagIndexResponse> IndexResumeAsync(
        Guid userId,
        Guid resumeId)
    {
        var resume =
            await _resumeRepository.GetByIdAsync(resumeId);

        if (resume is null)
        {
            throw new KeyNotFoundException(
                "Resume not found.");
        }

        if (resume.Candidate.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You do not have access to this resume.");
        }

        var text =
            await _textExtractor.ExtractTextAsync(
                resume.FilePath);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new InvalidOperationException(
                "No text could be extracted from the resume.");
        }

        var chunks = CreateChunks(text);

        await _ragRepository
            .DeleteByResumeIdAsync(resumeId);

        for (var i = 0; i < chunks.Count; i++)
        {
            var embedding =
                await _embeddingService
                    .GenerateEmbeddingAsync(chunks[i]);

            var chunk = new RagDocumentChunk
            {
                Id = Guid.NewGuid(),
                ResumeId = resumeId,
                ChunkText = chunks[i],
                Embedding = JsonSerializer.Serialize(
                    embedding),
                ChunkIndex = i,
                CreatedAt = DateTime.UtcNow
            };

            await _ragRepository.AddChunkAsync(chunk);
        }

        await _ragRepository.SaveChangesAsync();

        return new RagIndexResponse
        {
            ResumeId = resumeId,
            ChunksCreated = chunks.Count,
            Message = "Resume indexed successfully."
        };
    }

    public async Task<AskRagResponse> AskAsync(
        Guid userId,
        AskRagRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new ArgumentException(
                "Question cannot be empty.");
        }

        var resume =
            await _resumeRepository
                .GetByIdAsync(request.ResumeId);

        if (resume is null)
        {
            throw new KeyNotFoundException(
                "Resume not found.");
        }

        if (resume.Candidate.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You do not have access to this resume.");
        }

        var chunks =
            await _ragRepository
                .GetChunksByResumeIdAsync(
                    request.ResumeId);

        if (chunks.Count == 0)
        {
            throw new InvalidOperationException(
                "Resume has not been indexed yet.");
        }

        var questionEmbedding =
            await _embeddingService
                .GenerateEmbeddingAsync(
                    request.Question);

        var rankedChunks = chunks
            .Select(chunk =>
            {
                var embedding =
                    JsonSerializer.Deserialize<List<float>>(
                        chunk.Embedding)
                    ?? new List<float>();

                var similarity =
                    CosineSimilarity(
                        questionEmbedding,
                        embedding);

                return new
                {
                    Chunk = chunk,
                    Similarity = similarity
                };
            })
            .OrderByDescending(x => x.Similarity)
            .Take(3)
            .ToList();

        var context = new StringBuilder();

        foreach (var item in rankedChunks)
        {
            context.AppendLine(item.Chunk.ChunkText);
            context.AppendLine();
        }

        var prompt = $"""
        You are a career assistant.

        Answer the user's question using ONLY the resume
        information provided in the context.

        If the answer cannot be found in the context,
        say that the information is not available in
        the resume.

        Do not invent information.

        Resume Context:
        {context}

        User Question:
        {request.Question}

        Give a concise and professional answer.
        """;

        var answer =
            await _ollamaService
                .GenerateAsync(prompt);

        return new AskRagResponse
        {
            ResumeId = request.ResumeId,
            Question = request.Question,
            Answer = answer.Trim(),
            ChunksUsed = rankedChunks.Count
        };
    }

    private static List<string> CreateChunks(
        string text)
    {
        const int chunkSize = 1000;
        const int overlap = 150;

        var chunks = new List<string>();

        if (text.Length <= chunkSize)
        {
            chunks.Add(text.Trim());
            return chunks;
        }

        var start = 0;

        while (start < text.Length)
        {
            var length =
                Math.Min(
                    chunkSize,
                    text.Length - start);

            var chunk =
                text.Substring(start, length)
                    .Trim();

            if (!string.IsNullOrWhiteSpace(chunk))
            {
                chunks.Add(chunk);
            }

            if (start + length >= text.Length)
            {
                break;
            }

            start += chunkSize - overlap;
        }

        return chunks;
    }

    private static double CosineSimilarity(
        List<float> vectorA,
        List<float> vectorB)
    {
        if (vectorA.Count != vectorB.Count ||
            vectorA.Count == 0)
        {
            return 0;
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (var i = 0; i < vectorA.Count; i++)
        {
            dotProduct +=
                vectorA[i] * vectorB[i];

            magnitudeA +=
                vectorA[i] * vectorA[i];

            magnitudeB +=
                vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 ||
            magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct /
               (Math.Sqrt(magnitudeA) *
                Math.Sqrt(magnitudeB));
    }
}