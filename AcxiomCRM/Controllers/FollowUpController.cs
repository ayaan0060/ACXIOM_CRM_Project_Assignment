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
    public class FollowUpController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public FollowUpController(AppDbContext db, UserManager<AppUser> users, AuditService audit)
        {
            _db = db; _users = users; _audit = audit;
        }

        public async Task<IActionResult> Index(string status)
        {
            var userId = _users.GetUserId(User);
            var query = (User.IsInRole("Admin") || User.IsInRole("Manager"))
                ? _db.FollowUps.AsQueryable()
                : _db.FollowUps.Where(x => x.AssignedUserId == userId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            ViewBag.StatusFilter = status;
            ViewBag.OverdueCount = await _db.FollowUps.CountAsync(x => x.FollowUpDate < DateTime.Today && x.Status == "Planned");
            return View(await query.OrderBy(x => x.FollowUpDate).ToListAsync());
        }

        public IActionResult Create() => View(new FollowUp { FollowUpDate = DateTime.Today.AddDays(1) });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FollowUp m)
        {
            if (m.FollowUpDate.Date < DateTime.Today)
                ModelState.AddModelError("FollowUpDate", "Follow-up date cannot be earlier than today.");
            if (!ModelState.IsValid) return View(m);

            m.AssignedUserId = _users.GetUserId(User) ?? "";
            _db.FollowUps.Add(m);
            await _db.SaveChangesAsync();
            _audit.Log(m.AssignedUserId, "Create", "FollowUp", m.Id.ToString(), "", m.Subject);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var f = await _db.FollowUps.FindAsync(id);
            if (f == null) return NotFound();
            return View(f);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FollowUp m)
        {
            if (m.FollowUpDate.Date < DateTime.Today && m.Status == "Planned")
                ModelState.AddModelError("FollowUpDate", "Follow-up date cannot be earlier than today.");
            if (!ModelState.IsValid) return View(m);

            var existing = await _db.FollowUps.FindAsync(m.Id);
            if (existing == null) return NotFound();

            var old = existing.Status;
            existing.Subject = m.Subject; existing.FollowUpDate = m.FollowUpDate;
            existing.FollowUpType = m.FollowUpType; existing.Status = m.Status; existing.Notes = m.Notes;
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Update", "FollowUp", m.Id.ToString(), old, m.Status);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var f = await _db.FollowUps.FindAsync(id);
            if (f == null) return NotFound();
            _db.FollowUps.Remove(f);
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Delete", "FollowUp", id.ToString(), f.Subject, "");
            return RedirectToAction(nameof(Index));
        }
    }
}
