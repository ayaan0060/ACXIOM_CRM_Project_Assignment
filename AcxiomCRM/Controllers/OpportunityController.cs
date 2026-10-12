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
    public class OpportunityController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public OpportunityController(AppDbContext db, UserManager<AppUser> users, AuditService audit)
        {
            _db = db; _users = users; _audit = audit;
        }

        public async Task<IActionResult> Index(string search, string stage)
        {
            var userId = _users.GetUserId(User);
            var query = (User.IsInRole("Admin") || User.IsInRole("Manager"))
                ? _db.Opportunities.Include(x => x.Customer).AsQueryable()
                : _db.Opportunities.Include(x => x.Customer).Where(x => x.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search));
            if (!string.IsNullOrWhiteSpace(stage))
                query = query.Where(x => x.Stage == stage);

            ViewBag.Search = search; ViewBag.StageFilter = stage;
            return View(await query.OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Customers = await _db.Customers.ToListAsync();
            return View(new Opportunity { ExpectedCloseDate = DateTime.Now.AddDays(30) });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Opportunity m)
        {
            if (m.ExpectedCloseDate.Date < DateTime.Today)
                ModelState.AddModelError("ExpectedCloseDate", "Expected Close Date cannot be in the past.");
            if (!ModelState.IsValid) { ViewBag.Customers = await _db.Customers.ToListAsync(); return View(m); }

            m.OwnerId = _users.GetUserId(User) ?? "";
            m.CreatedDate = DateTime.Now;
            _db.Opportunities.Add(m);
            await _db.SaveChangesAsync();
            _audit.Log(m.OwnerId, "Create", "Opportunity", m.Id.ToString(), "", m.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var o = await _db.Opportunities.FindAsync(id);
            if (o == null) return NotFound();
            ViewBag.Customers = await _db.Customers.ToListAsync();
            return View(o);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Opportunity m)
        {
            if (m.ExpectedCloseDate.Date < DateTime.Today)
                ModelState.AddModelError("ExpectedCloseDate", "Expected Close Date cannot be in the past.");
            if (!ModelState.IsValid) { ViewBag.Customers = await _db.Customers.ToListAsync(); return View(m); }

            var existing = await _db.Opportunities.FindAsync(m.Id);
            if (existing == null) return NotFound();

            var old = existing.Stage;
            existing.Name = m.Name; existing.CustomerId = m.CustomerId; existing.Stage = m.Stage;
            existing.Amount = m.Amount; existing.Probability = m.Probability;
            existing.ExpectedCloseDate = m.ExpectedCloseDate; existing.Notes = m.Notes;
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Update", "Opportunity", m.Id.ToString(), old, m.Stage);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var o = await _db.Opportunities.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == id);
            if (o == null) return NotFound();
            return View(o);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var o = await _db.Opportunities.FindAsync(id);
            if (o == null) return NotFound();
            _db.Opportunities.Remove(o);
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Delete", "Opportunity", id.ToString(), o.Name, "");
            return RedirectToAction(nameof(Index));
        }
    }
}
