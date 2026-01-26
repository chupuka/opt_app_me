using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;

namespace ProForm.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MembershipsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MembershipsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Memberships
        public async Task<IActionResult> Index()
        {
            var memberships = await _context.Memberships
                .Include(m => m.Client)
                .Include(m => m.MembershipType)
                .ToListAsync();
            return View(memberships);
        }

        // GET: Memberships/Sell
        public async Task<IActionResult> Sell(int? clientId)
        {
            ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "FullName", clientId);
            ViewData["MembershipTypeId"] = new SelectList(
                await _context.MembershipTypes.Where(mt => mt.IsActive).ToListAsync(), 
                "MembershipTypeId", 
                "Name");
            return View();
        }

        // POST: Memberships/Sell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sell([Bind("ClientId,MembershipTypeId,StartDate,Amount,PaymentMethod")] SellMembershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var membershipType = await _context.MembershipTypes.FindAsync(model.MembershipTypeId);
                if (membershipType == null)
                {
                    return NotFound();
                }

                // Проверка суммы оплаты
                if (model.Amount < membershipType.Price)
                {
                    ModelState.AddModelError("Amount", "Сумма оплаты не может быть меньше стоимости абонемента.");
                    ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "FullName", model.ClientId);
                    ViewData["MembershipTypeId"] = new SelectList(
                        await _context.MembershipTypes.Where(mt => mt.IsActive).ToListAsync(), 
                        "MembershipTypeId", 
                        "Name",
                        model.MembershipTypeId);
                    return View(model);
                }

                // Деактивация старых абонементов клиента
                var oldMemberships = await _context.Memberships
                    .Where(m => m.ClientId == model.ClientId && m.IsActive)
                    .ToListAsync();
                foreach (var old in oldMemberships)
                {
                    old.IsActive = false;
                }

                // Создание нового абонемента
                var membership = new Membership
                {
                    ClientId = model.ClientId,
                    MembershipTypeId = model.MembershipTypeId,
                    StartDate = model.StartDate,
                    IsActive = true
                };

                // Расчет даты окончания или количества посещений
                if (membershipType.Category == MembershipCategory.TimeBased && membershipType.DurationDays.HasValue)
                {
                    membership.EndDate = model.StartDate.AddDays(membershipType.DurationDays.Value);
                }
                else if (membershipType.Category == MembershipCategory.VisitBased && membershipType.VisitCount.HasValue)
                {
                    membership.RemainingVisits = membershipType.VisitCount.Value;
                }

                _context.Add(membership);
                await _context.SaveChangesAsync();

                // Создание платежа
                var payment = new Payment
                {
                    ClientId = model.ClientId,
                    MembershipId = membership.MembershipId,
                    Amount = model.Amount,
                    PaymentMethod = model.PaymentMethod,
                    PaymentDate = DateTime.Now
                };
                _context.Add(payment);

                // Обновление статуса клиента
                var client = await _context.Clients.FindAsync(model.ClientId);
                if (client != null)
                {
                    client.Status = ClientStatus.Active;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(await _context.Clients.ToListAsync(), "ClientId", "FullName", model.ClientId);
            ViewData["MembershipTypeId"] = new SelectList(
                await _context.MembershipTypes.Where(mt => mt.IsActive).ToListAsync(), 
                "MembershipTypeId", 
                "Name",
                model.MembershipTypeId);
            return View(model);
        }

        // GET: Memberships/Freeze/5
        public async Task<IActionResult> Freeze(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var membership = await _context.Memberships
                .Include(m => m.Client)
                .Include(m => m.MembershipType)
                .FirstOrDefaultAsync(m => m.MembershipId == id);

            if (membership == null || !membership.IsActive)
            {
                return NotFound();
            }

            return View(new FreezeMembershipViewModel { MembershipId = id.Value });
        }

        // POST: Memberships/Freeze
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Freeze([Bind("MembershipId,FreezeStartDate,FreezeEndDate,Reason")] FreezeMembershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var membership = await _context.Memberships.FindAsync(model.MembershipId);
                if (membership == null || !membership.IsActive)
                {
                    return NotFound();
                }

                var freezeDays = (model.FreezeEndDate - model.FreezeStartDate).Days;
                if (freezeDays <= 0)
                {
                    ModelState.AddModelError("", "Период заморозки должен быть больше 0 дней.");
                    return View(model);
                }

                // Создание записи о заморозке
                var freeze = new MembershipFreeze
                {
                    MembershipId = model.MembershipId,
                    FreezeStartDate = model.FreezeStartDate,
                    FreezeEndDate = model.FreezeEndDate,
                    Reason = model.Reason
                };
                _context.Add(freeze);

                // Продление даты окончания абонемента
                if (membership.EndDate.HasValue)
                {
                    membership.EndDate = membership.EndDate.Value.AddDays(freezeDays);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }

    public class SellMembershipViewModel
    {
        public int ClientId { get; set; }
        public int MembershipTypeId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }

    public class FreezeMembershipViewModel
    {
        public int MembershipId { get; set; }
        public DateTime FreezeStartDate { get; set; }
        public DateTime FreezeEndDate { get; set; }
        public string? Reason { get; set; }
    }
}

