namespace AuditTrailExample.Models
{
    public class Employee
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public bool IsDeleted { get; internal set; }
    }
}
