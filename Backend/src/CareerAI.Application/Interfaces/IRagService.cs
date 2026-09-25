using CareerAI.Application.Features.RAG.DTOs;

namespace CareerAI.Application.Interfaces;

public interface IRagService
{
    Task<RagIndexResponse> IndexResumeAsync(
        Guid userId,
        Guid resumeId);

    Task<AskRagResponse> AskAsync(
        Guid userId,
        AskRagRequest request);
}