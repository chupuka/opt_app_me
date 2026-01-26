using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProForm.Data;
using ProForm.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace ProForm.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Reports/Financial
        public async Task<IActionResult> Financial(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var payments = await _context.Payments
                .Include(p => p.Client)
                .Include(p => p.Membership!)
                    .ThenInclude(m => m.MembershipType)
                .Where(p => p.PaymentDate >= start && p.PaymentDate <= end)
                .ToListAsync();

            var report = new FinancialReportViewModel
            {
                StartDate = start,
                EndDate = end,
                TotalAmount = payments.Sum(p => p.Amount),
                PaymentsByMethod = payments
                    .GroupBy(p => p.PaymentMethod)
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount)),
                PaymentsByMembershipType = payments
                    .Where(p => p.Membership != null)
                    .GroupBy(p => p.Membership!.MembershipType.Name)
                    .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount)),
                Payments = payments
            };

            return View(report);
        }

        // GET: Reports/Attendance
        public async Task<IActionResult> Attendance(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var registrations = await _context.ClassRegistrations
                .Include(r => r.Class)
                .Include(r => r.Client)
                .Where(r => r.Class.StartTime >= start && r.Class.StartTime <= end && r.Attended)
                .ToListAsync();

            var report = new AttendanceReportViewModel
            {
                StartDate = start,
                EndDate = end,
                TotalVisits = registrations.Count,
                UniqueVisitors = registrations.Select(r => r.ClientId).Distinct().Count(),
                TotalClasses = await _context.FitnessClasses
                    .CountAsync(c => c.StartTime >= start && c.StartTime <= end),
                PopularClasses = registrations
                    .GroupBy(r => r.Class.Title)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(report);
        }

        // GET: Reports/TrainerLoad
        public async Task<IActionResult> TrainerLoad(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var classes = await _context.FitnessClasses
                .Include(c => c.Trainer)
                .Where(c => c.StartTime >= start && c.StartTime <= end && c.Trainer != null)
                .ToListAsync();

            var report = new TrainerLoadReportViewModel
            {
                StartDate = start,
                EndDate = end,
                TrainerStats = classes
                    .GroupBy(c => c.Trainer!.TrainerId)
                    .Select(g => new TrainerStat
                    {
                        TrainerName = g.First().Trainer!.FullName,
                        ClassesCount = g.Count(),
                        TotalHours = g.Sum(c => (c.EndTime - c.StartTime).TotalHours)
                    })
                    .OrderByDescending(t => t.TotalHours)
                    .ToList()
            };

            return View(report);
        }

        // GET: Reports/ClubLoad
        public async Task<IActionResult> ClubLoad(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            var registrations = await _context.ClassRegistrations
                .Include(r => r.Class)
                .Where(r => r.Class.StartTime >= start && r.Class.StartTime <= end && r.Attended)
                .ToListAsync();

            var report = new ClubLoadReportViewModel
            {
                StartDate = start,
                EndDate = end,
                PeakDays = registrations
                    .GroupBy(r => r.Class.StartTime.Date)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count()),
                PeakHours = registrations
                    .GroupBy(r => r.Class.StartTime.Hour)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(report);
        }

        // POST: Reports/ExportPDF
        [HttpPost]
        public async Task<IActionResult> ExportPDF(string reportType, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-1);
            var end = endDate ?? DateTime.Today;

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 50, 50, 25, 25);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                document.Add(new Paragraph($"Отчет: {reportType}", titleFont));
                document.Add(new Paragraph($"Период: {start:dd.MM.yyyy} - {end:dd.MM.yyyy}", normalFont));
                document.Add(new Paragraph(" "));

                // Добавление данных отчета в зависимости от типа
                switch (reportType)
                {
                    case "Financial":
                        await AddFinancialReportData(document, start, end, headerFont, normalFont);
                        break;
                    case "Attendance":
                        await AddAttendanceReportData(document, start, end, headerFont, normalFont);
                        break;
                    // Добавить другие типы отчетов
                }

                document.Close();
                writer.Close();

                var bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"Report_{reportType}_{DateTime.Now:yyyyMMdd}.pdf");
            }
        }

        private async Task AddFinancialReportData(Document document, DateTime start, DateTime end, Font headerFont, Font normalFont)
        {
            var payments = await _context.Payments
                .Include(p => p.Client)
                .Include(p => p.Membership!)
                    .ThenInclude(m => m.MembershipType)
                .Where(p => p.PaymentDate >= start && p.PaymentDate <= end)
                .ToListAsync();

            document.Add(new Paragraph("Финансовый отчет", headerFont));
            document.Add(new Paragraph($"Общая сумма: {payments.Sum(p => p.Amount):C}", normalFont));
            document.Add(new Paragraph(" "));

            var table = new PdfPTable(4);
            table.AddCell(new PdfPCell(new Phrase("Дата", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Клиент", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Сумма", headerFont)));
            table.AddCell(new PdfPCell(new Phrase("Способ оплаты", headerFont)));

            foreach (var payment in payments)
            {
                table.AddCell(new PdfPCell(new Phrase(payment.PaymentDate.ToString("dd.MM.yyyy"), normalFont)));
                table.AddCell(new PdfPCell(new Phrase(payment.Client.FullName, normalFont)));
                table.AddCell(new PdfPCell(new Phrase(payment.Amount.ToString("C"), normalFont)));
                table.AddCell(new PdfPCell(new Phrase(payment.PaymentMethod.ToString(), normalFont)));
            }

            document.Add(table);
        }

        private async Task AddAttendanceReportData(Document document, DateTime start, DateTime end, Font headerFont, Font normalFont)
        {
            var registrations = await _context.ClassRegistrations
                .Include(r => r.Class)
                .Include(r => r.Client)
                .Where(r => r.Class.StartTime >= start && r.Class.StartTime <= end && r.Attended)
                .ToListAsync();

            document.Add(new Paragraph("Отчет по посещаемости", headerFont));
            document.Add(new Paragraph($"Всего посещений: {registrations.Count}", normalFont));
            document.Add(new Paragraph($"Уникальных посетителей: {registrations.Select(r => r.ClientId).Distinct().Count()}", normalFont));
        }
    }

    public class FinancialReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public Dictionary<PaymentMethod, decimal> PaymentsByMethod { get; set; } = new();
        public Dictionary<string, decimal> PaymentsByMembershipType { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();
    }

    public class AttendanceReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalVisits { get; set; }
        public int UniqueVisitors { get; set; }
        public int TotalClasses { get; set; }
        public Dictionary<string, int> PopularClasses { get; set; } = new();
    }

    public class TrainerLoadReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TrainerStat> TrainerStats { get; set; } = new();
    }

    public class TrainerStat
    {
        public string TrainerName { get; set; } = string.Empty;
        public int ClassesCount { get; set; }
        public double TotalHours { get; set; }
    }

    public class ClubLoadReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Dictionary<DateTime, int> PeakDays { get; set; } = new();
        public Dictionary<int, int> PeakHours { get; set; } = new();
    }
}

