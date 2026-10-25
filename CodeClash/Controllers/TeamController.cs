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

    public async Task<IActionResult> Create()
    {
        ViewBag.Competitions = new SelectList(
            await _context.Competitions.Where(c => c.IsOpenForRegistration).ToListAsync(),
            "Id",
            "Title"
        );
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TeamName,MentorName,ContactEmail,CompetitionId")] Team team)
    {
        if (ModelState.IsValid)
        {
            team.RegisteredOn = DateTime.UtcNow;
            _context.Add(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Competitions = new SelectList(
            await _context.Competitions.Where(c => c.IsOpenForRegistration).ToListAsync(),
            "Id",
            "Title",
            team.CompetitionId
        );
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,TeamName,MentorName,ContactEmail,CompetitionId,RegisteredOn")] Team team)
    {
        if (id != team.Id)
        {
            return NotFound();
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

        ViewBag.Competitions = new SelectList(
            await _context.Competitions.ToListAsync(),
            "Id",
            "Title",
            team.CompetitionId
        );
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