using System.Threading.Tasks;
using CapstoneReview.Repository.Data.DbContexts;
using CapstoneReview.Repository.Entities;
using CapstoneReview.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneReview.Repository.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;
    
    public Task<User?> GetUserByEmailAsync(string email)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
