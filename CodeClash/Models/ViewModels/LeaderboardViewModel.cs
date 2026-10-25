using CodeClash.Models;

namespace CodeClash.Models.ViewModels;

public class LeaderboardViewModel
{
    public int? SelectedCompetitionId { get; set; }
    public List<Competition> Competitions { get; set; } = new();
    public List<LeaderboardItemViewModel> RankedTeams { get; set; } = new();
}

public class LeaderboardItemViewModel
{
    public int Rank { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string MentorName { get; set; } = string.Empty;
    public string CompetitionTitle { get; set; } = string.Empty;
    public string? ProjectTitle { get; set; }
    public string? GitHubUrl { get; set; }
    public int? Score { get; set; }
}