using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IOllamaService _ollamaService;

    public AIController(IOllamaService ollamaService)
    {
        _ollamaService = ollamaService;
    }

    [HttpPost("test")]
    public async Task<IActionResult> Test()
    {
        var prompt = """
                     Explain what an LLM is in simple terms.
                     Keep the answer within 100 words.
                     """;

        var response = await _ollamaService.GenerateAsync(prompt);

        return Ok(new
        {
            message = "Ollama connection successful.",
            response = response
        });
    }
}