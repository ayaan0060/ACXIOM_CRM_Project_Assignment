using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class LeadController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public LeadController(AppDbContext db, UserManager<AppUser> users, AuditService audit)
        {
            _db = db; _users = users; _audit = audit;
        }

        public async Task<IActionResult> Index(string search, string status)
        {
            var userId = _users.GetUserId(User);
            var query = (User.IsInRole("Admin") || User.IsInRole("Manager"))
                ? _db.Leads.AsQueryable()
                : _db.Leads.Where(x => x.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.CompanyName.Contains(search));
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            ViewBag.Search = search; ViewBag.StatusFilter = status;
            return View(await query.OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        public IActionResult Create() => View(new Lead());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead m)
        {
            if (!ModelState.IsValid) return View(m);
            m.OwnerId = _users.GetUserId(User) ?? "";
            m.CreatedDate = DateTime.Now;
            _db.Leads.Add(m);
            await _db.SaveChangesAsync();
            _audit.Log(m.OwnerId, "Create", "Lead", m.Id.ToString(), "", m.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var l = await _db.Leads.FindAsync(id);
            if (l == null) return NotFound();
            return View(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Lead m)
        {
            if (!ModelState.IsValid) return View(m);
            var existing = await _db.Leads.FindAsync(m.Id);
            if (existing == null) return NotFound();

            var old = existing.Status;
            existing.Name = m.Name; existing.Email = m.Email; existing.Phone = m.Phone;
            existing.CompanyName = m.CompanyName; existing.Source = m.Source;
            existing.Status = m.Status; existing.Priority = m.Priority; existing.ExpectedValue = m.ExpectedValue;
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Update", "Lead", m.Id.ToString(), old, m.Status);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var l = await _db.Leads.FindAsync(id);
            if (l == null) return NotFound();
            return View(l);
        }

        public async Task<IActionResult> Convert(int id)
        {
            var lead = await _db.Leads.FindAsync(id);
            if (lead == null) return NotFound();
            if (lead.Status != "Qualified") { TempData["Error"] = "Only Qualified leads can be converted."; return RedirectToAction(nameof(Index)); }

            var customer = new Customer
            {
                Name = lead.Name, Email = lead.Email, Phone = lead.Phone,
                CompanyName = lead.CompanyName, OwnerId = lead.OwnerId, CreatedDate = DateTime.Now
            };
            _db.Customers.Add(customer);
            lead.IsConverted = true; lead.Status = "Converted";
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Convert", "Lead", id.ToString(), "Qualified", "Converted");
            return RedirectToAction("Index", "Customer");
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var l = await _db.Leads.FindAsync(id);
            if (l == null) return NotFound();
            _db.Leads.Remove(l);
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Delete", "Lead", id.ToString(), l.Name, "");
            return RedirectToAction(nameof(Index));
        }
    }
}
