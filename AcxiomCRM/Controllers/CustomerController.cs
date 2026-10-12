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
    public class CustomerController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public CustomerController(AppDbContext db, UserManager<AppUser> users, AuditService audit)
        {
            _db = db; _users = users; _audit = audit;
        }

        public async Task<IActionResult> Index(string search)
        {
            var userId = _users.GetUserId(User);
            var query = (User.IsInRole("Admin") || User.IsInRole("Manager"))
                ? _db.Customers.AsQueryable()
                : _db.Customers.Where(x => x.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.Email.Contains(search) || x.Phone.Contains(search) || x.CompanyName.Contains(search));

            ViewBag.Search = search;
            return View(await query.OrderByDescending(x => x.CreatedDate).ToListAsync());
        }

        public IActionResult Create() => View(new Customer());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer m)
        {
            if (!ModelState.IsValid) return View(m);

            bool emailExists = await _db.Customers.AnyAsync(x => x.Email == m.Email);
            bool phoneExists = await _db.Customers.AnyAsync(x => x.Phone == m.Phone);
            if (emailExists) { ModelState.AddModelError("Email", "Email already exists."); return View(m); }
            if (phoneExists) { ModelState.AddModelError("Phone", "Phone already exists."); return View(m); }

            m.OwnerId = _users.GetUserId(User) ?? "";
            m.CreatedDate = DateTime.Now;
            _db.Customers.Add(m);
            await _db.SaveChangesAsync();
            _audit.Log(m.OwnerId, "Create", "Customer", m.Id.ToString(), "", m.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer m)
        {
            if (!ModelState.IsValid) return View(m);
            var existing = await _db.Customers.FindAsync(m.Id);
            if (existing == null) return NotFound();

            bool emailExists = await _db.Customers.AnyAsync(x => x.Email == m.Email && x.Id != m.Id);
            bool phoneExists = await _db.Customers.AnyAsync(x => x.Phone == m.Phone && x.Id != m.Id);
            if (emailExists) { ModelState.AddModelError("Email", "Email already exists."); return View(m); }
            if (phoneExists) { ModelState.AddModelError("Phone", "Phone already exists."); return View(m); }

            var old = existing.Name;
            existing.Name = m.Name; existing.Email = m.Email; existing.Phone = m.Phone;
            existing.Address = m.Address; existing.CompanyName = m.CompanyName; existing.Status = m.Status;
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Update", "Customer", m.Id.ToString(), old, m.Name);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            return View(c);
        }

        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync();
            _audit.Log(_users.GetUserId(User) ?? "", "Delete", "Customer", id.ToString(), c.Name, "");
            return RedirectToAction(nameof(Index));
        }
    }
}
