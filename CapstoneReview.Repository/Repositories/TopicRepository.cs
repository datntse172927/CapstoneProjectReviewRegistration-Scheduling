using CapstoneReview.Repository.Data.DbContexts;
using CapstoneReview.Repository.Entities;
using CapstoneReview.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneReview.Repository.Repositories;

public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _context;

    public TopicRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Topic>> GetAllAsync()
    {
        return await _context.Topics
            .Include(t => t.Lecturer)
            .ToListAsync();
    }

    public async Task<Topic?> GetByIdAsync(int id)
    {
        return await _context.Topics
            .Include(t => t.Lecturer)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public void Add(Topic topic)
    {
        _context.Topics.Add(topic);
    }

    public void Remove(Topic topic)
    {
        _context.Topics.Remove(topic);
    }
}