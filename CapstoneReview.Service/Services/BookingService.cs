using CapstoneReview.Service.DTOs;
using CapstoneReview.Repository.Entities;
using CapstoneReview.Repository.Interfaces;
using CapstoneReview.Service.Exceptions;
using CapstoneReview.Service.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapstoneReview.Service.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AvailableSlotResponse>> GetAvailableSlotsAsync()
    {
        var slots = await _unitOfWork.Slots.GetAvailableSlotsAsync();
        return slots.Select(s => new AvailableSlotResponse
        {
            SlotId = s.Id,
            ReviewRound = s.ReviewRound,
            Room = s.Room,
            StartTime = s.StartTime,
            EndTime = s.EndTime
        }).ToList();
    }

    public async Task BookTeamAsync(TeamBookingRequest request)
    {
        // Truy ngược: RollNumber → Student → Team
        var team = await _unitOfWork.Teams.GetTeamByLeaderRollNumberAsync(request.LeaderRollNumber);
        if (team == null) throw new BusinessRuleException("Không tìm thấy nhóm nào có Leader với mã sinh viên này.");

        var topic = team.Topic;
        if (topic == null)
        {
            topic = await _unitOfWork.Teams.GetTopicByTeamIdAsync(team.Id);
        }
        if (topic == null) throw new BusinessRuleException("Team does not have an assigned topic.");

        var slotTopics = new List<SlotTopic>();
        foreach (var slotId in request.SlotIds)
        {
            var slot = await _unitOfWork.Slots.GetSlotByIdAsync(slotId);
            if (slot == null) throw new BusinessRuleException($"Slot {slotId} not found.");
            if (slot.SlotTopics.Count >= 3) throw new BusinessRuleException($"Slot {slotId} already has maximum 3 topics.");
            
            var alreadyBooked = await _unitOfWork.Slots.HasTopicBookedSlotAsync(topic.Id, slotId);
            if (!alreadyBooked)
            {
                slotTopics.Add(new SlotTopic { SlotId = slotId, TopicId = topic.Id });
            }
        }

        if (slotTopics.Any())
        {
            await _unitOfWork.Slots.AddSlotTopicsAsync(slotTopics);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task BookLecturerAsync(LecturerBookingRequest request)
    {
        var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(request.LecturerId);
        if (lecturer == null) throw new BusinessRuleException("Lecturer not found.");

        var slotLecturers = new List<SlotLecturer>();
        foreach (var slotId in request.SlotIds)
        {
            var slot = await _unitOfWork.Slots.GetSlotByIdAsync(slotId);
            if (slot == null) throw new BusinessRuleException($"Slot {slotId} not found.");
            if (slot.SlotLecturers.Count >= 2) throw new BusinessRuleException($"Slot {slotId} already has maximum 2 reviewers.");

            var alreadyBooked = await _unitOfWork.Slots.HasLecturerBookedSlotAsync(request.LecturerId, slotId);
            if (!alreadyBooked)
            {
                slotLecturers.Add(new SlotLecturer { SlotId = slotId, LecturerId = request.LecturerId });
            }
        }

        if (slotLecturers.Any())
        {
            await _unitOfWork.Slots.AddSlotLecturersAsync(slotLecturers);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ConfigureLecturerAsync(int lecturerId, ModeratorConfigLecturerRequest request)
    {
        if (request.MinSlot < 0) throw new BusinessRuleException("MinSlot must be >= 0.");
        if (request.MaxSlot < request.MinSlot) throw new BusinessRuleException("MaxSlot must be >= MinSlot.");

        var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(lecturerId);
        if (lecturer == null) throw new BusinessRuleException("Lecturer not found.");

        lecturer.MinSlot = request.MinSlot;
        lecturer.MaxSlot = request.MaxSlot;

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CreateSlotAsync(CreateSlotDto request)
    {
        if (request.EndTime <= request.StartTime)
            throw new BusinessRuleException("EndTime must be greater than StartTime.");

        if (request.RegistrationDeadline > request.StartTime)
            throw new BusinessRuleException("RegistrationDeadline must be before StartTime.");

        var slot = new Slot
        {
            ReviewRound = request.ReviewRound,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Room = request.Room,
            RegistrationDeadline = request.RegistrationDeadline
        };

        // Using direct context since SlotRepository lacks Add method
        // Wait, does SlotRepository have Add? I will add it using DbContext temporarily or use _unitOfWork if it has methods
        // Better yet, just add AddSlot(Slot slot) to ISlotRepository
        _unitOfWork.Slots.AddSlot(slot);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CancelTeamBookingAsync(CancelTeamBookingRequest request)
{
    var team = await _unitOfWork.Teams.GetTeamByLeaderRollNumberAsync(request.LeaderRollNumber);
    if (team == null)
        throw new BusinessRuleException("Không tìm thấy nhóm nào có Leader với mã sinh viên này.");

    var topic = team.Topic ?? await _unitOfWork.Teams.GetTopicByTeamIdAsync(team.Id);
    if (topic == null)
        throw new BusinessRuleException("Team does not have an assigned topic.");

    foreach (var slotId in request.SlotIds.Distinct())
    {
        var slot = await _unitOfWork.Slots.GetSlotByIdAsync(slotId);
        if (slot == null)
            throw new BusinessRuleException($"Slot {slotId} not found.");

        var booking = slot.SlotTopics.FirstOrDefault(x => x.TopicId == topic.Id);
        if (booking != null)
        {
            slot.SlotTopics.Remove(booking);
        }
    }

    await _unitOfWork.SaveChangesAsync();
}

public async Task CancelLecturerBookingAsync(CancelLecturerBookingRequest request)
{
    var lecturer = await _unitOfWork.Lecturers.GetByIdAsync(request.LecturerId);
    if (lecturer == null)
        throw new BusinessRuleException("Lecturer not found.");

    foreach (var slotId in request.SlotIds.Distinct())
    {
        var slot = await _unitOfWork.Slots.GetSlotByIdAsync(slotId);
        if (slot == null)
            throw new BusinessRuleException($"Slot {slotId} not found.");

        var booking = slot.SlotLecturers.FirstOrDefault(x => x.LecturerId == request.LecturerId);
        if (booking != null)
        {
            slot.SlotLecturers.Remove(booking);
        }
    }

    await _unitOfWork.SaveChangesAsync();
}

public async Task UpdateTeamBookingAsync(UpdateTeamBookingRequest request)
{
    await CancelTeamBookingAsync(new CancelTeamBookingRequest
    {
        LeaderRollNumber = request.LeaderRollNumber,
        SlotIds = request.OldSlotIds
    });

    await BookTeamAsync(new TeamBookingRequest
    {
        LeaderRollNumber = request.LeaderRollNumber,
        SlotIds = request.NewSlotIds
    });
}

public async Task UpdateLecturerBookingAsync(UpdateLecturerBookingRequest request)
{
    await CancelLecturerBookingAsync(new CancelLecturerBookingRequest
    {
        LecturerId = request.LecturerId,
        SlotIds = request.OldSlotIds
    });

    await BookLecturerAsync(new LecturerBookingRequest
    {
        LecturerId = request.LecturerId,
        SlotIds = request.NewSlotIds
    });
}

}
