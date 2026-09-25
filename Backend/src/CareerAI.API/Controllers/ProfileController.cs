using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CareerAI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    [HttpGet]
    public IActionResult GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(ClaimTypes.Name);

        var email = User.FindFirstValue(ClaimTypes.Email);

        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            Message = "JWT authentication is working.",
            UserId = userId,
            Email = email,
            Role = role
        });
    }
}