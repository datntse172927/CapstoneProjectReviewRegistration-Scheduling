using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CapstoneReview.Repository.Interfaces;
using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneReview.Service.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(request.Email);
        
        if (user == null || user.Password != request.Password)
        {
            throw new BusinessRuleException("Sai email hoặc mật khẩu.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        // Cần đảm bảo có key trong appsettings.json
        var keyString = _configuration["JwtConfig:Key"] ?? "CapstoneReviewProjectSecretKeyDungChoDevLocalTestThoiNhe123!!";
        var key = Encoding.ASCII.GetBytes(keyString);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.FullName)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        return new LoginResponse
        {
            Token = tokenHandler.WriteToken(token),
            FullName = user.FullName,
            Role = user.Role
        };
    }
}
