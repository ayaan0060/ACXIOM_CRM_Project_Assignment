using AcxiomCRM.Data;
using AcxiomCRM.Models;

namespace AcxiomCRM.Services
{
    public class AuditService
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public AuditService(AppDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public void Log(string userId, string action, string entity, string recordId, string old = "", string newVal = "")
        {
            _db.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                EntityName = entity,
                RecordId = recordId,
                OldValue = old,
                NewValue = newVal,
                IpAddress = _http.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? ""
            });
            _db.SaveChanges();
        }
    }
}
