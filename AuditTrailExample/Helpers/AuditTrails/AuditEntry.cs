﻿using AuditTrailExample.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuditTrailExample.Helpers.AuditTrails
{
    public class AuditEntry
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }
        public string? FieldName { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        [ForeignKey("Audit")]
        public int AuditId { get; set; }
        public Audit Audit { get; set; }
    }
}
