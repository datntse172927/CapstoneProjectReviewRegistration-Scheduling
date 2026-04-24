using CapstoneReview.Repository.Entities;
using CapstoneReview.Repository.Interfaces;
using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace CapstoneReview.Service.Services;

public class TopicService : ITopicService
{
    private readonly IUnitOfWork _unitOfWork;

    public TopicService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<TopicResponse>> GetAllTopicsAsync()
    {
        var topics = await _unitOfWork.Topics.GetAllAsync();

        return topics.Select(t => new TopicResponse
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            LecturerId = t.LecturerId,
            LecturerName = t.Lecturer?.FullName ?? "",
            TeamId = t.Team?.Id,
            TeamName = t.Team?.TeamName ?? ""
        }).ToList();
    }

    public async Task<TopicResponse?> GetTopicByIdAsync(int id)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null) return null;

        return new TopicResponse
        {
            Id = topic.Id,
            Title = topic.Title,
            Description = topic.Description,
            LecturerId = topic.LecturerId,
            LecturerName = topic.Lecturer?.FullName ?? "",
            TeamId = topic.Team?.Id,
            TeamName = topic.Team?.TeamName ?? ""
        };
    }

    public async Task<TopicResponse> CreateTopicAsync(CreateTopicRequest request)
    {
        var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(request.LecturerId);
        if (lecturer == null)
            throw new BusinessRuleException("Lecturer not found.");

        var topic = new Topic
        {
            Title = request.Title,
            Description = request.Description,
            LecturerId = request.LecturerId,
        };

        _unitOfWork.Topics.Add(topic);
        await _unitOfWork.SaveChangesAsync();

        return new TopicResponse
        {
            Id = topic.Id,
            Title = topic.Title,
            Description = topic.Description,
            LecturerId = topic.LecturerId,
            LecturerName = lecturer.FullName,
            TeamId = topic.Team?.Id,
            TeamName = topic.Team?.TeamName ?? ""
        };
    }

    public async Task<TopicResponse> UpdateTopicAsync(int id, UpdateTopicRequest request)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null)
            throw new BusinessRuleException("Topic not found.");

        var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(request.LecturerId);
        if (lecturer == null)
            throw new BusinessRuleException("Lecturer not found.");

        topic.Title = request.Title;
        topic.Description = request.Description;
        topic.LecturerId = request.LecturerId;

        await _unitOfWork.SaveChangesAsync();

        return new TopicResponse
        {
            Id = topic.Id,
            Title = topic.Title,
            Description = topic.Description,
            LecturerId = topic.LecturerId,
            LecturerName = lecturer.FullName,
            TeamId = topic.Team?.Id,
            TeamName = topic.Team?.TeamName ?? ""
        };
    }

    public async Task DeleteTopicAsync(int id)
    {
        var topic = await _unitOfWork.Topics.GetByIdAsync(id);
        if (topic == null)
            throw new BusinessRuleException("Topic not found.");

        _unitOfWork.Topics.Remove(topic);
        await _unitOfWork.SaveChangesAsync();
    }
}