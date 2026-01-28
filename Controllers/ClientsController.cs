using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;
using System.ComponentModel.DataAnnotations;

namespace ProForm.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string? searchString, ClientStatus? status, DateTime? expirationDate)
        {
            var clients = _context.Clients
                .Include(c => c.Memberships.Where(m => m.IsActive))
                    .ThenInclude(m => m.MembershipType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(c => 
                    c.FullName.Contains(searchString) ||
                    c.Phone.Contains(searchString) ||
                    (c.Email != null && c.Email.Contains(searchString)));
            }

            if (status.HasValue)
            {
                clients = clients.Where(c => c.Status == status.Value);
            }

            if (expirationDate.HasValue)
            {
                clients = clients.Where(c => 
                    c.Memberships.Any(m => m.IsActive && 
                        m.EndDate.HasValue && 
                        m.EndDate.Value.Date == expirationDate.Value.Date));
            }

            return View(await clients.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Memberships)
                    .ThenInclude(m => m.MembershipType)
                .Include(c => c.ClassRegistrations)
                    .ThenInclude(r => r.Class)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(m => m.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FullName,DateOfBirth,Phone,Email,Status")] Client client)
        {
            // Валидация модели вручную (для unit-тестов)
            var validationContext = new ValidationContext(client, null, null);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(client, validationContext, validationResults, true))
            {
                foreach (var error in validationResults)
                {
                    foreach (var memberName in error.MemberNames)
                    {
                        ModelState.AddModelError(memberName, error.ErrorMessage ?? "");
                    }
                }
            }

            // Проверка уникальности телефона
            if (!string.IsNullOrEmpty(client.Phone) && 
                await _context.Clients.AnyAsync(c => c.Phone == client.Phone))
            {
                ModelState.AddModelError("Phone", "Клиент с таким телефоном уже существует.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,FullName,DateOfBirth,Phone,Email,Status")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            // Проверка на активные абонементы
            var hasActiveMemberships = await _context.Memberships
                .AnyAsync(m => m.ClientId == id && m.IsActive);

            if (hasActiveMemberships)
            {
                ViewBag.ErrorMessage = "Невозможно удалить клиента с активными абонементами.";
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                // Проверка на активные абонементы
                var hasActiveMemberships = await _context.Memberships
                    .AnyAsync(m => m.ClientId == id && m.IsActive);

                if (hasActiveMemberships)
                {
                    ModelState.AddModelError("", "Невозможно удалить клиента с активными абонементами.");
                    return View(client);
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/GetAll (для AJAX)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _context.Clients
                .Select(c => new { c.ClientId, c.FullName })
                .ToListAsync();
            return Json(clients);
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}

