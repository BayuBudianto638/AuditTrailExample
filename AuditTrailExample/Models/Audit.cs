using AuditTrailExample.Helpers.AuditTrails;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuditTrailExample.Models
{
    public class Audit
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Operation { get; set; }
        public string TableName { get; set; }
        public int RecordId { get; set; }
        public IEnumerable<AuditEntry> Changes { get; set; }
        public DateTime? ChangeDate { get; set; }
        [ForeignKey("ChangedBy")]
        public int ChangedById { get; set; }
        public int ChangedBy { get; set; }
    }
}
