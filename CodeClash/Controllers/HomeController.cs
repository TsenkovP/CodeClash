using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeClash.Data;
using CodeClash.Models;
using CodeClash.Models.ViewModels;

namespace CodeClash.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
{
    var now = DateTime.UtcNow.Date;

    // 1. Статистически показатели
    int activeCompetitions = await _context.Competitions
        .CountAsync(c => c.IsOpenForRegistration && c.EventDate >= now);

    int totalTeams = await _context.Teams.CountAsync();
    int totalSubmissions = await _context.ProjectSubmissions.CountAsync();

    var scores = await _context.ProjectSubmissions
        .Where(p => p.Score.HasValue)
        .Select(p => p.Score!.Value)
        .ToListAsync();

    double averageScore = scores.Any() ? Math.Round(scores.Average(), 1) : 0.0;

    // 2. Предстоящо събитие
    var nextCompetition = await _context.Competitions
        .Include(c => c.Teams)
        .Where(c => c.IsOpenForRegistration && c.EventDate >= now)
        .OrderBy(c => c.EventDate)
        .FirstOrDefaultAsync();

    // 3. Топ 3 проекта (материализираме първо със ToListAsync, след което номерираме)
    var topTeamsFromDb = await _context.Teams
        .Include(t => t.Competition)
        .Include(t => t.Submission)
        .Where(t => t.Submission != null && t.Submission.Score.HasValue)
        .OrderByDescending(t => t.Submission!.Score)
        .Take(3)
        .ToListAsync();

    var topProjects = topTeamsFromDb
        .Select((t, index) => new LeaderboardItemViewModel
        {
            Rank = index + 1,
            TeamName = t.TeamName,
            MentorName = t.MentorName,
            CompetitionTitle = t.Competition != null ? t.Competition.Title : "—",
            ProjectTitle = t.Submission!.ProjectTitle,
            GitHubUrl = t.Submission.GitHubUrl,
            Score = t.Submission.Score
        })
        .ToList();

    var viewModel = new HomeDashboardViewModel
    {
        ActiveCompetitionsCount = activeCompetitions,
        TotalTeamsCount = totalTeams,
        SubmissionsCount = totalSubmissions,
        AverageScore = averageScore,
        NextUpcomingCompetition = nextCompetition,
        TopProjects = topProjects
    };

    return View(viewModel);
}

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}