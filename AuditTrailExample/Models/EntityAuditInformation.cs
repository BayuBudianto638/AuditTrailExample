using AuditTrailExample.Helpers.AuditTrails;
using Microsoft.EntityFrameworkCore;

namespace AuditTrailExample.Models
{
    public class EntityAuditInformation
    {
        public dynamic Entity { get; set; }
        public string TableName { get; set; }

        public EntityState State { get; set; }

        public List<AuditEntry> Changes { get; set; }

        public bool IsDeleteChanged { get; set; }

        public string OperationType
        {
            get
            {
                switch (State)
                {
                    case EntityState.Added:
                        return "Create";
                    case EntityState.Modified:
                        string deleteOrRestore = Entity.IsDeleted ? "Delete" : "Restore";
                        return IsDeleteChanged ? deleteOrRestore : "Update";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}
