using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeClash.Data;
using CodeClash.Models.ViewModels;

namespace CodeClash.Controllers;

public class LeaderboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaderboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? competitionId)
    {
        var competitions = await _context.Competitions
            .AsNoTracking()
            .ToListAsync();

        var query = _context.Teams
            .Include(t => t.Competition)
            .Include(t => t.Submission)
            .AsNoTracking()
            .AsQueryable();

        if (competitionId.HasValue)
        {
            query = query.Where(t => t.CompetitionId == competitionId.Value);
        }

        var teams = await query.ToListAsync();

        var rankedList = teams
            .OrderByDescending(t => t.Submission?.Score ?? -1)
            .ThenBy(t => t.TeamName)
            .Select((t, index) => new LeaderboardItemViewModel
            {
                Rank = index + 1,
                TeamName = t.TeamName,
                MentorName = t.MentorName,
                CompetitionTitle = t.Competition?.Title ?? "—",
                ProjectTitle = t.Submission?.ProjectTitle,
                GitHubUrl = t.Submission?.GitHubUrl,
                Score = t.Submission?.Score
            })
            .ToList();

        var viewModel = new LeaderboardViewModel
        {
            SelectedCompetitionId = competitionId,
            Competitions = competitions,
            RankedTeams = rankedList
        };

        return View(viewModel);
    }
}