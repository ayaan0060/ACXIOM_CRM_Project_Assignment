using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AcxiomCRM.Models;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;

        public DashboardController(AppDbContext db, UserManager<AppUser> users)
        {
            _db = db; _users = users;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _users.GetUserId(User);
            bool isAdmin = User.IsInRole("Admin");
            bool isManager = User.IsInRole("Manager");

            var custQ = isAdmin || isManager ? _db.Customers : _db.Customers.Where(x => x.OwnerId == userId);
            var leadQ = isAdmin || isManager ? _db.Leads : _db.Leads.Where(x => x.OwnerId == userId);
            var oppQ = isAdmin || isManager ? _db.Opportunities : _db.Opportunities.Where(x => x.OwnerId == userId);

            ViewBag.TotalCustomers = await custQ.CountAsync();
            ViewBag.TotalLeads = await leadQ.CountAsync();
            ViewBag.OpenLeads = await leadQ.Where(x => x.Status != "Converted" && x.Status != "Lost").CountAsync();
            ViewBag.TotalOpportunities = await oppQ.CountAsync();
            ViewBag.OpenOpportunities = await oppQ.Where(x => x.Stage != "Won" && x.Stage != "Lost").CountAsync();
            ViewBag.WonOpportunities = await oppQ.Where(x => x.Stage == "Won").CountAsync();
            ViewBag.LostOpportunities = await oppQ.Where(x => x.Stage == "Lost").CountAsync();
            ViewBag.PipelineValue = await oppQ.Where(x => x.Stage != "Won" && x.Stage != "Lost").SumAsync(x => (decimal?)x.Amount) ?? 0;

            // Chart data
            var leadStatuses = new[] { "New", "Contacted", "Qualified", "Lost", "Converted" };
            ViewBag.LeadStatusLabels = leadStatuses;
            ViewBag.LeadStatusData = leadStatuses.Select(s => leadQ.Count(l => l.Status == s)).ToArray();

            var stages = new[] { "Qualification", "Proposal", "Negotiation", "Won", "Lost" };
            ViewBag.StageLabels = stages;
            ViewBag.StageData = stages.Select(s => oppQ.Count(o => o.Stage == s)).ToArray();

            return View();
        }
    }
}
