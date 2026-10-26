using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeClash.Data;
using CodeClash.Models;

namespace CodeClash.Controllers;

public class ProjectSubmissionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectSubmissionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /ProjectSubmissions
    public async Task<IActionResult> Index()
    {
        var submissions = await _context.ProjectSubmissions
            .Include(p => p.Team)
            .ThenInclude(t => t.Competition)
            .AsNoTracking()
            .ToListAsync();

        return View(submissions);
    }

    // GET: /ProjectSubmissions/Create
    public async Task<IActionResult> Create()
    {
        // Зареждаме само отбори, които все още нямат предаден проект
        var submittedTeamIds = await _context.ProjectSubmissions
            .Select(s => s.TeamId)
            .ToListAsync();

        var availableTeams = await _context.Teams
            .Where(t => !submittedTeamIds.Contains(t.Id))
            .ToListAsync();

        ViewBag.Teams = new SelectList(availableTeams, "Id", "TeamName");
        return View();
    }

    // POST: /ProjectSubmissions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProjectTitle,GitHubUrl,Score,TeamId")] ProjectSubmission submission)
    {
        // Проверка: Има ли вече предаден проект за този отбор
        bool alreadySubmitted = await _context.ProjectSubmissions
            .AnyAsync(p => p.TeamId == submission.TeamId);

        if (alreadySubmitted)
        {
            ModelState.AddModelError("TeamId", "Избраният отбор вече има предаден проект.");
        }

        // Проверка за валиден GitHub URL формат (ако е попълнен)
        if (!string.IsNullOrWhiteSpace(submission.GitHubUrl) && 
            !Uri.TryCreate(submission.GitHubUrl, UriKind.Absolute, out _))
        {
            ModelState.AddModelError("GitHubUrl", "Моля, въведете валиден пълен URL адрес (напр. https://github.com/...).");
        }

        if (ModelState.IsValid)
        {
            submission.SubmittedOn = DateTime.UtcNow;
            _context.Add(submission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Ако има грешка, презареждаме наличните отбори, за да не се чупи формата
        var submittedTeamIds = await _context.ProjectSubmissions
            .Where(s => s.TeamId != submission.TeamId)
            .Select(s => s.TeamId)
            .ToListAsync();

        var availableTeams = await _context.Teams
            .Where(t => !submittedTeamIds.Contains(t.Id))
            .ToListAsync();

        ViewBag.Teams = new SelectList(availableTeams, "Id", "TeamName", submission.TeamId);
        return View(submission);
    }

    // GET: /ProjectSubmissions/Score/5 (форма за оценяване от жури)
    public async Task<IActionResult> Score(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var submission = await _context.ProjectSubmissions
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (submission == null)
        {
            return NotFound();
        }

        return View(submission);
    }

    // POST: /ProjectSubmissions/Score/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Score(int id, int? score)
    {
        var submission = await _context.ProjectSubmissions.FindAsync(id);
        if (submission == null)
        {
            return NotFound();
        }

        submission.Score = score;
        _context.Update(submission);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}