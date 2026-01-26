using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;

namespace ProForm.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        // Занятия на сегодня
        var todayClasses = await _context.FitnessClasses
            .Include(c => c.Trainer)
            .Where(c => c.StartTime.Date == today)
            .OrderBy(c => c.StartTime)
            .ToListAsync();

        // Работающие тренеры сегодня
        var workingTrainers = await _context.Trainers
            .Where(t => t.Status == TrainerStatus.Active && 
                       t.Classes.Any(c => c.StartTime.Date == today))
            .ToListAsync();

        // Истекающие абонементы (через 1 и 3 дня)
        var expiringMemberships = await _context.Memberships
            .Include(m => m.Client)
            .Include(m => m.MembershipType)
            .Where(m => m.IsActive && 
                       m.EndDate.HasValue && 
                       m.EndDate.Value >= today && 
                       m.EndDate.Value <= today.AddDays(3))
            .OrderBy(m => m.EndDate)
            .ToListAsync();

        ViewBag.TodayClasses = todayClasses;
        ViewBag.WorkingTrainers = workingTrainers;
        ViewBag.ExpiringMemberships = expiringMemberships;

        return View();
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
