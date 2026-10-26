using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CodeClash.Data;
using CodeClash.Models;

namespace CodeClash.Controllers;

public class TeamsController : Controller
{
    private readonly ApplicationDbContext _context;

    public TeamsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _context.Teams
            .Include(t => t.Competition)
            .Include(t => t.Submission)
            .AsNoTracking()
            .ToListAsync();

        return View(teams);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .Include(t => t.Competition)
            .Include(t => t.Submission)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    // GET: Teams/Create или Teams/Create?competitionId=2
    public async Task<IActionResult> Create(int? competitionId)
    {
        // Зареждаме само състезания, които са отворени за записване
        var openCompetitions = await _context.Competitions
            .Where(c => c.IsOpenForRegistration && c.EventDate >= DateTime.UtcNow.Date)
            .AsNoTracking()
            .ToListAsync();

        // Ако идваме от конкретно състезание, проверяваме дали то съществува и е отворено
        if (competitionId.HasValue)
        {
            var targetCompetition = await _context.Competitions.FindAsync(competitionId.Value);
            if (targetCompetition != null && targetCompetition.IsOpenForRegistration)
            {
                ViewBag.PreselectedCompetition = targetCompetition;
                ViewBag.CompetitionId = new SelectList(openCompetitions, "Id", "Title", competitionId.Value);
                return View(new Team { CompetitionId = competitionId.Value });
            }
        }

        ViewBag.CompetitionId = new SelectList(openCompetitions, "Id", "Title");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeamName,MentorName,ContactEmail,CompetitionId,MaxTeamMembers")] Team team)
    {
        var competition = await _context.Competitions.FindAsync(team.CompetitionId);
    
        // Проверка 1: Съществува ли и отворено ли е състезанието
        if (competition == null || !competition.IsOpenForRegistration)
        {
            ModelState.AddModelError("CompetitionId", "Регистрацията за това състезание е затворена или не съществува.");
        }

        // Проверка 2: Уникалност на името на отбора в рамките на това състезание
        bool teamNameExists = await _context.Teams.AnyAsync(t => 
            t.CompetitionId == team.CompetitionId && 
            t.TeamName.Trim().ToLower() == team.TeamName.Trim().ToLower());

        if (teamNameExists)
        {
            ModelState.AddModelError("TeamName", $"Вече съществува отбор с име \"{team.TeamName}\" в избраното състезание.");
        }

        if (ModelState.IsValid)
        {
            team.RegisteredOn = DateTime.UtcNow;
            _context.Add(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // При грешка зареждаме отново списъка с активни състезания
        var openCompetitions = await _context.Competitions
            .Where(c => c.IsOpenForRegistration && c.EventDate >= DateTime.UtcNow.Date)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.CompetitionId = new SelectList(openCompetitions, "Id", "Title", team.CompetitionId);
        return View(team);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams.FindAsync(id);
        if (team == null)
        {
            return NotFound();
        }

        ViewBag.Competitions = new SelectList(
            await _context.Competitions.ToListAsync(),
            "Id",
            "Title",
            team.CompetitionId
        );
        return View(team);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,TeamName,MentorName,ContactEmail,CompetitionId,RegisteredOn,MaxTeamMembers")] Team team)
    {
        if (id != team.Id)
        {
            return NotFound();
        }

        // Проверка: Има ли друг отбор със същото име в същото състезание
        bool duplicateExists = await _context.Teams.AnyAsync(t => 
            t.Id != id &&
            t.CompetitionId == team.CompetitionId && 
            t.TeamName.Trim().ToLower() == team.TeamName.Trim().ToLower());

        if (duplicateExists)
        {
            ModelState.AddModelError("TeamName", $"Вече съществува друг отбор с име \"{team.TeamName}\" в това състезание.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(team);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Teams.AnyAsync(e => e.Id == team.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.CompetitionId = new SelectList(_context.Competitions, "Id", "Title", team.CompetitionId);
        return View(team);
    }

   public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var team = await _context.Teams
            .Include(t => t.Competition)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (team == null)
        {
            return NotFound();
        }

        return View(team);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var team = await _context.Teams.FindAsync(id);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}