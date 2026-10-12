using AcxiomCRM.Data;
using AcxiomCRM.DTOs;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    [ApiController, Route("api"), Authorize]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<AppUser> _users;

        public ApiController(AppDbContext db, UserManager<AppUser> users)
        {
            _db = db; _users = users;
        }

        // Customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers() =>
            Ok(await _db.Customers.Select(c => new { c.Id, c.Name, c.Email, c.Phone, c.Status }).ToListAsync());

        [HttpGet("customers/{id}")]
        public async Task<IActionResult> GetCustomer(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            return Ok(new { c.Id, c.Name, c.Email, c.Phone, c.CompanyName, c.Status });
        }

        [HttpPost("customers")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool emailExists = await _db.Customers.AnyAsync(x => x.Email == dto.Email);
            if (emailExists) return Conflict(new { message = "Email already exists." });

            var c = new Customer { Name = dto.Name, Email = dto.Email, Phone = dto.Phone, CompanyName = dto.CompanyName, OwnerId = _users.GetUserId(User) ?? "" };
            _db.Customers.Add(c);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomer), new { id = c.Id }, new { c.Id, c.Name, c.Email });
        }

        [HttpPut("customers/{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            c.Name = dto.Name; c.Email = dto.Email; c.Phone = dto.Phone; c.CompanyName = dto.CompanyName;
            await _db.SaveChangesAsync();
            return Ok(new { c.Id, c.Name, c.Email });
        }

        [HttpDelete("customers/{id}"), Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var c = await _db.Customers.FindAsync(id);
            if (c == null) return NotFound();
            _db.Customers.Remove(c);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Deleted." });
        }

        // Leads
        [HttpGet("leads")]
        public async Task<IActionResult> GetLeads() =>
            Ok(await _db.Leads.Select(l => new { l.Id, l.Name, l.Email, l.Status, l.Source }).ToListAsync());

        [HttpPost("leads")]
        public async Task<IActionResult> CreateLead([FromBody] LeadDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var l = new Lead { Name = dto.Name, Email = dto.Email, Phone = dto.Phone, Source = dto.Source, Status = "New", OwnerId = _users.GetUserId(User) ?? "" };
            _db.Leads.Add(l);
            await _db.SaveChangesAsync();
            return Created("", new { l.Id, l.Name });
        }

        // Opportunities
        [HttpGet("opportunities")]
        public async Task<IActionResult> GetOpportunities() =>
            Ok(await _db.Opportunities.Select(o => new { o.Id, o.Name, o.Stage, o.Amount, o.Probability }).ToListAsync());

        [HttpPost("opportunities")]
        public async Task<IActionResult> CreateOpportunity([FromBody] OpportunityDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.ExpectedCloseDate.Date < DateTime.Today)
                return BadRequest(new { message = "Expected Close Date cannot be in the past." });

            var o = new Opportunity { Name = dto.Name, CustomerId = dto.CustomerId, Stage = dto.Stage, Amount = dto.Amount, Probability = dto.Probability, ExpectedCloseDate = dto.ExpectedCloseDate, OwnerId = _users.GetUserId(User) ?? "" };
            _db.Opportunities.Add(o);
            await _db.SaveChangesAsync();
            return Created("", new { o.Id, o.Name });
        }

        // Reports pipeline
        [HttpGet("reports/pipeline"), Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Pipeline()
        {
            var data = await _db.Opportunities.GroupBy(o => o.Stage)
                .Select(g => new { Stage = g.Key, Count = g.Count(), Total = g.Sum(x => x.Amount) }).ToListAsync();
            return Ok(data);
        }
    }
}
