using CapstoneReview.Repository.Entities;

namespace CapstoneReview.Repository.Interfaces;

public interface ITeamRepository
{
    Task<List<Team>> GetAllTeamsAsync();
    Task<Team?> GetTeamByIdAsync(int teamId);
    Task<Team?> GetTeamByLeaderRollNumberAsync(string rollNumber);
    Task<Topic?> GetTopicByTeamIdAsync(int teamId);
}
