using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;

namespace ProForm.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScheduleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Schedule
        public async Task<IActionResult> Index()
        {
            ViewData["TrainerId"] = new SelectList(
                await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                "TrainerId", 
                "FullName");
            return View();
        }

        // GET: Schedule/GetEvents
        [HttpGet]
        public async Task<IActionResult> GetEvents(DateTime start, DateTime end)
        {
            var events = await _context.FitnessClasses
                .Include(c => c.Trainer)
                .Where(c => c.StartTime >= start && c.EndTime <= end)
                .Select(c => new
                {
                    id = c.ClassId,
                    title = c.Title,
                    start = c.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end = c.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                    trainer = c.Trainer != null ? c.Trainer.FullName : "",
                    hall = c.Hall ?? "",
                    maxParticipants = c.MaxParticipants
                })
                .ToListAsync();

            return Json(events);
        }

        // GET: Schedule/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClass = await _context.FitnessClasses
                .Include(c => c.Trainer)
                .Include(c => c.Registrations)
                    .ThenInclude(r => r.Client)
                .FirstOrDefaultAsync(m => m.ClassId == id);

            if (fitnessClass == null)
            {
                return NotFound();
            }

            return View(fitnessClass);
        }

        // GET: Schedule/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewData["TrainerId"] = new SelectList(
                await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                "TrainerId", 
                "FullName");
            return View();
        }

        // POST: Schedule/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Type,Title,TrainerId,Hall,StartTime,EndTime,MaxParticipants")] FitnessClass fitnessClass)
        {
            if (ModelState.IsValid)
            {
                // Проверка даты
                if (fitnessClass.StartTime < DateTime.Now)
                {
                    ModelState.AddModelError("StartTime", "Дата начала не может быть в прошлом.");
                    ViewData["TrainerId"] = new SelectList(
                        await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                        "TrainerId", 
                        "FullName",
                        fitnessClass.TrainerId);
                    return View(fitnessClass);
                }

                // Проверка максимального количества участников
                if (fitnessClass.MaxParticipants.HasValue && fitnessClass.MaxParticipants.Value <= 0)
                {
                    ModelState.AddModelError("MaxParticipants", "Максимальное количество участников должно быть больше 0.");
                    ViewData["TrainerId"] = new SelectList(
                        await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                        "TrainerId", 
                        "FullName",
                        fitnessClass.TrainerId);
                    return View(fitnessClass);
                }

                _context.Add(fitnessClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(
                await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                "TrainerId", 
                "FullName",
                fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        // POST: Schedule/UpdateEvent
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent(int id, DateTime start, DateTime end)
        {
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass == null)
            {
                return NotFound();
            }

            fitnessClass.StartTime = start;
            fitnessClass.EndTime = end;

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        // POST: Schedule/RegisterClient
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterClient(int classId, int clientId)
        {
            var fitnessClass = await _context.FitnessClasses
                .Include(c => c.Registrations)
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (fitnessClass == null)
            {
                return NotFound();
            }

            // Проверка наличия активного абонемента
            var hasActiveMembership = await _context.Memberships
                .AnyAsync(m => m.ClientId == clientId && m.IsActive);

            if (!hasActiveMembership)
            {
                return Json(new { success = false, message = "У клиента нет активного абонемента." });
            }

            // Проверка свободных мест
            if (fitnessClass.MaxParticipants.HasValue && 
                fitnessClass.Registrations.Count >= fitnessClass.MaxParticipants.Value)
            {
                return Json(new { success = false, message = "Нет свободных мест на занятие." });
            }

            // Проверка на дублирование записи
            if (fitnessClass.Registrations.Any(r => r.ClientId == clientId))
            {
                return Json(new { success = false, message = "Клиент уже записан на это занятие." });
            }

            var registration = new ClassRegistration
            {
                ClassId = classId,
                ClientId = clientId,
                RegistrationDate = DateTime.Now
            };

            _context.Add(registration);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: Schedule/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass == null)
            {
                return NotFound();
            }

            ViewData["TrainerId"] = new SelectList(
                await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                "TrainerId", 
                "FullName",
                fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        // POST: Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,Type,Title,TrainerId,Hall,StartTime,EndTime,MaxParticipants")] FitnessClass fitnessClass)
        {
            if (id != fitnessClass.ClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fitnessClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FitnessClassExists(fitnessClass.ClassId))
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

            ViewData["TrainerId"] = new SelectList(
                await _context.Trainers.Where(t => t.Status == TrainerStatus.Active).ToListAsync(), 
                "TrainerId", 
                "FullName",
                fitnessClass.TrainerId);
            return View(fitnessClass);
        }

        // GET: Schedule/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fitnessClass = await _context.FitnessClasses
                .Include(c => c.Trainer)
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (fitnessClass == null)
            {
                return NotFound();
            }

            return View(fitnessClass);
        }

        // POST: Schedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fitnessClass = await _context.FitnessClasses.FindAsync(id);
            if (fitnessClass != null)
            {
                _context.FitnessClasses.Remove(fitnessClass);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool FitnessClassExists(int id)
        {
            return _context.FitnessClasses.Any(e => e.ClassId == id);
        }

        // POST: Schedule/MarkAttendance
        [HttpPost]
        [Authorize(Roles = "Admin,Trainer")]
        public async Task<IActionResult> MarkAttendance(int registrationId, bool attended)
        {
            var registration = await _context.ClassRegistrations
                .Include(r => r.Client)
                .Include(r => r.Class)
                .FirstOrDefaultAsync(r => r.RegistrationId == registrationId);

            if (registration == null)
            {
                return NotFound();
            }

            registration.Attended = attended;

            // Если посещение отмечено и у клиента разовый абонемент - списать посещение
            if (attended)
            {
                var membership = await _context.Memberships
                    .Include(m => m.MembershipType)
                    .FirstOrDefaultAsync(m => 
                        m.ClientId == registration.ClientId && 
                        m.IsActive &&
                        m.MembershipType.Category == MembershipCategory.VisitBased);

                if (membership != null && membership.RemainingVisits.HasValue)
                {
                    if (membership.RemainingVisits.Value > 0)
                    {
                        membership.RemainingVisits--;
                    }
                    else
                    {
                        return Json(new { success = false, message = "У клиента закончились посещения." });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}

