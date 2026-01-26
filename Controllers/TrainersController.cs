using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;

namespace ProForm.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers
                .Include(t => t.Schedules)
                .ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Schedules)
                .Include(t => t.Classes)
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FullName,Specialization,Phone,Email,Status")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                // Проверка уникальности email
                if (!string.IsNullOrEmpty(trainer.Email) && 
                    await _context.Trainers.AnyAsync(t => t.Email == trainer.Email))
                {
                    ModelState.AddModelError("Email", "Тренер с таким email уже существует.");
                    return View(trainer);
                }

                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,FullName,Specialization,Phone,Email,Status")] Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Проверка уникальности email
                    if (!string.IsNullOrEmpty(trainer.Email) && 
                        await _context.Trainers.AnyAsync(t => t.Email == trainer.Email && t.TrainerId != id))
                    {
                        ModelState.AddModelError("Email", "Тренер с таким email уже существует.");
                        return View(trainer);
                    }

                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            // Проверка на назначенные занятия
            var hasClasses = await _context.FitnessClasses
                .AnyAsync(c => c.TrainerId == id);

            if (hasClasses)
            {
                ViewBag.ErrorMessage = "Невозможно удалить тренера с назначенными занятиями.";
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // Проверка на назначенные занятия
                var hasClasses = await _context.FitnessClasses
                    .AnyAsync(c => c.TrainerId == id);

                if (hasClasses)
                {
                    ModelState.AddModelError("", "Невозможно удалить тренера с назначенными занятиями.");
                    return View(trainer);
                }

                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.TrainerId == id);
        }
    }
}

