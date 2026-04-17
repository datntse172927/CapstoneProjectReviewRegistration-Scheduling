using CapstoneReview.Service.DTOs;

namespace CapstoneReview.Service.Interfaces;

public interface ITopicService
{
    Task<List<TopicResponse>> GetAllTopicsAsync();
    Task<TopicResponse?> GetTopicByIdAsync(int id);
    Task<TopicResponse> CreateTopicAsync(CreateTopicRequest request);
    Task<TopicResponse> UpdateTopicAsync(int id, UpdateTopicRequest request);
    Task DeleteTopicAsync(int id);
}