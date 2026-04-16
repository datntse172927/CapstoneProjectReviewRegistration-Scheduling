using System;
using System.Threading.Tasks;
using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneReview.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (BusinessRuleException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Hệ thống đang gặp sự cố khi đăng nhập.", details = ex.Message });
        }
    }
}
