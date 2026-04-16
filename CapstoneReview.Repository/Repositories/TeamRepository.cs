using CapstoneReview.Repository.Data.DbContexts;
using CapstoneReview.Repository.Entities;
using CapstoneReview.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneReview.Repository.Repositories;

public class TeamRepository : ITeamRepository
{
    private readonly ApplicationDbContext _context;
    public TeamRepository(ApplicationDbContext context) => _context = context;
    public Task<List<Team>> GetAllTeamsAsync() => _context.Teams.Include(t => t.Students).ToListAsync();
    public Task<Team?> GetTeamByIdAsync(int teamId) => _context.Teams.FirstOrDefaultAsync(t => t.Id == teamId);
    
    public async Task<Team?> GetTeamByLeaderRollNumberAsync(string rollNumber)
    {
        // Tìm Student theo RollNumber
        var student = await _context.Students.FirstOrDefaultAsync(s => s.RollNumber == rollNumber);
        if (student == null) return null;
        
        // Tìm Team mà student này là Leader
        return await _context.Teams
            .Include(t => t.Topic)
            .FirstOrDefaultAsync(t => t.LeaderId == student.Id);
    }
    
    public async Task<Topic?> GetTopicByTeamIdAsync(int teamId)
    {
        var team = await _context.Teams.Include(t => t.Topic).FirstOrDefaultAsync(t => t.Id == teamId);
        return team?.Topic;
    }
}
