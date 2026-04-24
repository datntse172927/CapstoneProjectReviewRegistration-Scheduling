namespace CapstoneReview.Service.DTOs;

public class AvailableSlotResponse
{
    public int SlotId { get; set; }
    public int ReviewRound { get; set; }
    public System.DateTime StartTime { get; set; }
    public System.DateTime EndTime { get; set; }
    public string Room { get; set; } = string.Empty;
}

public class TeamBookingRequest
{
    public string LeaderRollNumber { get; set; } = null!;
    public System.Collections.Generic.List<int> SlotIds { get; set; } = new();
}

public class LecturerBookingRequest
{
    public int LecturerId { get; set; }
    public System.Collections.Generic.List<int> SlotIds { get; set; } = new();
}

public class ModeratorConfigLecturerRequest
{
    public int MinSlot { get; set; }
    public int MaxSlot { get; set; }
}

public class CreateSlotDto
{
    public int ReviewRound { get; set; }
    public System.DateTime StartTime { get; set; }
    public System.DateTime EndTime { get; set; }
    public string Room { get; set; } = string.Empty;
    public System.DateTime RegistrationDeadline { get; set; }
}

public class CancelTeamBookingRequest
{
    public string LeaderRollNumber { get; set; } = null!;
    public List<int> SlotIds { get; set; } = new();
}

public class CancelLecturerBookingRequest
{
    public int LecturerId { get; set; }
    public List<int> SlotIds { get; set; } = new();
}

public class UpdateTeamBookingRequest
{
    public string LeaderRollNumber { get; set; } = null!;
    public List<int> OldSlotIds { get; set; } = new();
    public List<int> NewSlotIds { get; set; } = new();
}

public class UpdateLecturerBookingRequest
{
    public int LecturerId { get; set; }
    public List<int> OldSlotIds { get; set; } = new();
    public List<int> NewSlotIds { get; set; } = new();
}