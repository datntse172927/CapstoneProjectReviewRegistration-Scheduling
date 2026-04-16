using System.ComponentModel.DataAnnotations;

namespace CapstoneReview.Repository.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = null!;
    
    [Required]
    [MaxLength(50)]
    // LECTURER, STUDENT, MODERATOR
    public string Role { get; set; } = null!;
    
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
}
