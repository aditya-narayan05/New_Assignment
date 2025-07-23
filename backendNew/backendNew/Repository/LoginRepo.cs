using backendNew.DataAccessLayer;
using backendNew.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class LoginRepo : ILogin
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginRepo(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string?> AuthenticateAsync(LoginDTO loginDto)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.Email == loginDto.Email && u.Password == loginDto.Password);

        if (user == null) return null;

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing"))
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}