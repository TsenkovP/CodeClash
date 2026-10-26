using CodeClash.Models;

namespace CodeClash.Models.ViewModels;

public class HomeDashboardViewModel
{
    public int ActiveCompetitionsCount { get; set; }
    public int TotalTeamsCount { get; set; }
    public int SubmissionsCount { get; set; }
    public double AverageScore { get; set; }

    public Competition? NextUpcomingCompetition { get; set; }
    public List<LeaderboardItemViewModel> TopProjects { get; set; } = new();
}