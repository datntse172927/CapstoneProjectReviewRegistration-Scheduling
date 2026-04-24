namespace CapstoneReview.Service.DTOs;

public class TopicResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LecturerId { get; set; }
    public string LecturerName { get; set; } = string.Empty;

    public int? TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;

}

public class CreateTopicRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LecturerId { get; set; }
}

public class UpdateTopicRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int LecturerId { get; set; }
}