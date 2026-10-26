using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeClash.Data;
using CodeClash.Models;

namespace CodeClash.Controllers;

public class CompetitionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CompetitionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var competitions = await _context.Competitions
            .Include(c => c.Teams)
            .AsNoTracking()
            .OrderByDescending(c => c.EventDate)
            .ToListAsync();

        return View(competitions);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var competition = await _context.Competitions
            .Include(c => c.Teams)
                .ThenInclude(t => t.Submission)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (competition == null)
        {
            return NotFound();
        }

        return View(competition);
    }

    public IActionResult Create()
    {
        return View(new Competition
        {
            EventDate = DateTime.Today.AddDays(14),
            IsOpenForRegistration = true,
            MaxTeamMembers = 4
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description,EventDate,IsOpenForRegistration,MaxTeamMembers")] Competition competition)
    {
        if (ModelState.IsValid)
        {
            _context.Add(competition);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(competition);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var competition = await _context.Competitions.FindAsync(id);
        if (competition == null)
        {
            return NotFound();
        }
        return View(competition);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDate,IsOpenForRegistration,MaxTeamMembers")] Competition competition)
    {
        if (id != competition.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(competition);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Competitions.AnyAsync(e => e.Id == competition.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(competition);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var competition = await _context.Competitions
            .Include(c => c.Teams)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (competition == null)
        {
            return NotFound();
        }

        return View(competition);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var competition = await _context.Competitions.FindAsync(id);
        if (competition != null)
        {
            _context.Competitions.Remove(competition);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}