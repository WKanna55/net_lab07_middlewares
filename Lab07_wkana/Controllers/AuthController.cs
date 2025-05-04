using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lab07_wkana.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Lab07_wkana.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpPost("loginUser")]
    public IActionResult LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);
        
        // Verificar usuario y contraseña (simplificado)
        if (loginDto.Username == "user" && loginDto.Password == "user")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, loginDto.Username),
                new Claim(ClaimTypes.Role, "User"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }
        return Unauthorized();
    }

    [HttpPost("loginAdmin")]
    public IActionResult LoginAdmin([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);
        
        // Verificar usuario y contraseña (simplificado)
        if (loginDto.Username == "admin" && loginDto.Password == "admin")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, loginDto.Username),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);
            
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }
        return Unauthorized();
    }
    
    [HttpGet("admin")]
    public IActionResult GetAdminData()
    {
        return Ok("Datos solo para administradores");
    }
}