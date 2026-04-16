using System.Threading.Tasks;
using CapstoneReview.Repository.Entities;

namespace CapstoneReview.Repository.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
}
