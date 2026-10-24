namespace CodeClash.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class ProjectSubmission
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string ProjectTitle { get; set; } = null!;

    [Url]
    public string? GitHubUrl { get; set; }

    [Range(0, 100)]
    public int? Score { get; set; }

    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }
    public Team? Team { get; set; }
}