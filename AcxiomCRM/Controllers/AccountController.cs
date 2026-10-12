using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcxiomCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signIn;
        private readonly UserManager<AppUser> _users;
        private readonly AuditService _audit;

        public AccountController(SignInManager<AppUser> signIn, UserManager<AppUser> users, AuditService audit)
        {
            _signIn = signIn; _users = users; _audit = audit;
        }

        [HttpGet] public IActionResult Login() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _signIn.PasswordSignInAsync(email, password, false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                var user = await _users.FindByEmailAsync(email);
                _audit.Log(user?.Id ?? "", "Login", "Auth", email);
                return RedirectToAction("Index", "Dashboard");
            }
            if (result.IsLockedOut)
            {
                _audit.Log("", "Lockout", "Auth", email);
                ModelState.AddModelError("", "Account locked. Try again later.");
            }
            else
            {
                _audit.Log("", "FailedLogin", "Auth", email);
                ModelState.AddModelError("", "Invalid credentials.");
            }
            return View();
        }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string password, string role = "SalesExecutive")
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }
            var user = new AppUser { UserName = email, Email = email, FullName = fullName, IsActive = true };
            var result = await _users.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _users.AddToRoleAsync(user, role);
                await _signIn.SignInAsync(user, false);
                _audit.Log(user.Id, "Register", "Auth", email);
                return RedirectToAction("Index", "Dashboard");
            }
            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var user = await _users.GetUserAsync(User);
            _audit.Log(user?.Id ?? "", "Logout", "Auth", user?.Email ?? "");
            await _signIn.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied() => View();
    }
}
