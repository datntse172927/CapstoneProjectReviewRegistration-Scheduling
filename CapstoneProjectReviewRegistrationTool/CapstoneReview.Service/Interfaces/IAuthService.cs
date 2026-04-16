using System.Threading.Tasks;
using CapstoneReview.Service.DTOs;

namespace CapstoneReview.Service.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
}
