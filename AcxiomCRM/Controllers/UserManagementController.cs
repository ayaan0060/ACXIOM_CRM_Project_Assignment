using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public UserManagementController(UserManager<AppUser> users, AuditService audit)
        {
            _users = users; _audit = audit;
        }

        public IActionResult Index(string search)
        {
            var query = string.IsNullOrWhiteSpace(search)
                ? _users.Users.ToList()
                : _users.Users.Where(x => x.Email!.Contains(search) || x.FullName.Contains(search)).ToList();
            ViewBag.Search = search;
            return View(query);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string fullName, string email, string password, string role)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }
            var user = new AppUser { UserName = email, Email = email, FullName = fullName, IsActive = true };
            var result = await _users.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _users.AddToRoleAsync(user, role);
                _audit.Log(_users.GetUserId(User) ?? "", "CreateUser", "User", email, "", role);
                return RedirectToAction(nameof(Index));
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = !user.IsActive;
            await _users.UpdateAsync(user);
            _audit.Log(_users.GetUserId(User) ?? "", "ToggleActive", "User", id, "", user.IsActive.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}
