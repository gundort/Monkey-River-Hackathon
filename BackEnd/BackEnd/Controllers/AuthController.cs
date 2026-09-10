using BackEnd.DTOs;
using BackEnd.Models;
using BackEnd.Services;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMongoDbContext _mongoDbContext;
        private readonly IJwtService _jwtService;

        public AuthController(IMongoDbContext mongoDbContext, IJwtService jwtService)
        {
            _mongoDbContext = mongoDbContext;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingUser = await _mongoDbContext.Users
                .Find(u => u.Email == registerDto.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoDbContext.Users.InsertOneAsync(user);
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _mongoDbContext.Users
                .Find(u => u.Email == loginDto.Email)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName
            };

            return Ok(response);
        }
    }
}

