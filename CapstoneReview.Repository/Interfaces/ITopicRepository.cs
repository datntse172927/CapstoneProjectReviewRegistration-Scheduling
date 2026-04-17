using CapstoneReview.Repository.Entities;

namespace CapstoneReview.Repository.Interfaces;

public interface ITopicRepository
{
    Task<List<Topic>> GetAllAsync();
    Task<Topic?> GetByIdAsync(int id);
    void Add(Topic topic);
    void Remove(Topic topic);
}