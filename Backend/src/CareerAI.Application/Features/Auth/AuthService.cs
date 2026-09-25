using CareerAI.Application.Common.Security;
using CareerAI.Application.Features.Auth.DTOs;
using CareerAI.Application.Interfaces;
using CareerAI.Domain.Entities;
using CareerAI.Domain.Enums;

namespace CareerAI.Application.Features.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IRecruiterRepository _recruiterRepository;
    private readonly PasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;

    public AuthService(
    IUserRepository userRepository,
    ICandidateRepository candidateRepository,
    IRecruiterRepository recruiterRepository,
    PasswordHasher passwordHasher,
    IJwtService jwtService)
    {
        _userRepository = userRepository;
        _candidateRepository = candidateRepository;
        _recruiterRepository = recruiterRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
    }
    // ========================================
    // Register
    // ========================================

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Check whether email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            throw new Exception("Email is already registered.");
        }

        // Only Candidate and Recruiter can register publicly
        UserRole role;

        if (request.Role == (int)UserRole.Candidate)
        {
            role = UserRole.Candidate;
        }
        else if (request.Role == (int)UserRole.Recruiter)
        {
            role = UserRole.Recruiter;
        }
        else
        {
            throw new Exception("Invalid registration role.");
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),

            FirstName = request.FirstName,

            LastName = request.LastName,

            Email = request.Email,

            PasswordHash = _passwordHasher.HashPassword(
                request.Password),

            Role = role,

            CreatedAt = DateTime.UtcNow
        };

        // Save user
        await _userRepository.AddAsync(user);

        await _userRepository.SaveChangesAsync();

        // Create Candidate profile
        if (role == UserRole.Candidate)
        {
            var candidate = new Candidate
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                PhoneNumber = null,

                Location = null,

                Bio = null,

                DateOfBirth = null
            };

            await _candidateRepository.AddAsync(candidate);

            await _candidateRepository.SaveChangesAsync();
        }
        else if (role == UserRole.Recruiter)
        {
            var recruiter = new Recruiter
            {
                Id = Guid.NewGuid(),

                UserId = user.Id,

                CompanyName = string.Empty,

                CompanyDescription = null,

                CompanyWebsite = null,

                Location = null
            };

            await _recruiterRepository.AddAsync(recruiter);

            await _recruiterRepository.SaveChangesAsync();
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.Role.ToString());

        // Return response
        return new AuthResponse
        {
            Token = token,

            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            Role = user.Role.ToString()
        };
    }

    // ========================================
    // Login
    // ========================================

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Find user by email
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new Exception("Invalid email or password.");
        }

        // Verify password
        var passwordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            throw new Exception("Invalid email or password.");
        }

        // Generate JWT token
        var token = _jwtService.GenerateToken(
            user.Id,
            user.Email,
            user.Role.ToString());

        // Return response
        return new AuthResponse
        {
            Token = token,

            UserId = user.Id,

            FirstName = user.FirstName,

            LastName = user.LastName,

            Email = user.Email,

            Role = user.Role.ToString()
        };
    }
}