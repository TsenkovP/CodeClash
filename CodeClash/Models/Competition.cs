using System.ComponentModel.DataAnnotations;
namespace CodeClash.Models;

public class Competition
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = null!;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = null!;

    public DateTime EventDate { get; set; }

    [Range(1, 10)]
    public int MaxTeamMembers { get; set; }

    public bool IsOpenForRegistration { get; set; }

    public ICollection<Team> Teams { get; set; } = new List<Team>();
}