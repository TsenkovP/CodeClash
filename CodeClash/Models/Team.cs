using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CodeClash.Models;

public class Team
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string TeamName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string MentorName { get; set; } = null!;

    [Required]
    [EmailAddress]
    public string ContactEmail { get; set; } = null!;

    public DateTime RegisteredOn { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(Competition))]
    public int CompetitionId { get; set; }
    public Competition? Competition { get; set; }

    public ProjectSubmission? Submission { get; set; }
}