using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly AppDbContext _db;
        public AuditLogController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index(string action, string entity)
        {
            var query = _db.AuditLogs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.Action == action);
            if (!string.IsNullOrWhiteSpace(entity)) query = query.Where(x => x.EntityName == entity);
            ViewBag.ActionFilter = action; ViewBag.EntityFilter = entity;
            return View(await query.OrderByDescending(x => x.CreatedDate).Take(200).ToListAsync());
        }
    }
}
