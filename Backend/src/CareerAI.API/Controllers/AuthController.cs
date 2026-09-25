using CareerAI.Application.Features.Auth.DTOs;
using CareerAI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // ========================================
    // Register
    // POST: api/auth/register
    // ========================================

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return Ok(result);
    }

    // ========================================
    // Login
    // POST: api/auth/login
    // ========================================

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }
}