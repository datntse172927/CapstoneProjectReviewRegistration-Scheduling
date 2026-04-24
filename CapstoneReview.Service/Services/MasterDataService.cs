using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapstoneReview.Repository.Interfaces;
using CapstoneReview.Service.DTOs;
using CapstoneReview.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CapstoneReview.Service.Services;

public class MasterDataService : IMasterDataService
{
    private readonly IUnitOfWork _unitOfWork;

    public MasterDataService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<MasterLecturerDto>> GetAllLecturersAsync()
    {
        var lecturers = await _unitOfWork.Lecturers.GetAllLecturersWithDetailsAsync();
        return lecturers.Select(l => new MasterLecturerDto
        {
            Id = l.Id,
            FullName = l.FullName,
            Email = l.Email,
            MinSlot = l.MinSlot,
            MaxSlot = l.MaxSlot
        }).ToList();
    }

    public async Task<List<MasterTeamDto>> GetAllTeamsAsync()
    {
        var teams = await _unitOfWork.Teams.GetAllTeamsAsync();
        return teams.Select(t => new MasterTeamDto
        {
            Id = t.Id,
            TeamName = t.TeamName,
            LeaderId = t.LeaderId,
            LeaderRollNumber = t.Students?.FirstOrDefault(s => s.Id == t.LeaderId)?.RollNumber ?? "N/A",
            TopicId = t.TopicId,
            MemberCount = t.Students?.Count ?? 0
        }).ToList();
    }
}
