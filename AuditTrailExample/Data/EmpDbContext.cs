using AuditTrailExample.Helpers.AuditTrails;
using AuditTrailExample.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;

namespace AuditTrailExample.Data
{
    public class EmpDbContext : DbContext
    {
        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public EmpDbContext(DbContextOptions<EmpDbContext> options)
            : base(options)
        {
        }

        protected EmpDbContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        private List<EntityAuditInformation> BeforeSaveChanges()
        {
            List<EntityAuditInformation> entityAuditInformation = new();

            foreach (EntityEntry entityEntry in ChangeTracker.Entries())
            {
                dynamic entity = entityEntry.Entity;
                bool isAdd = entityEntry.State == EntityState.Added;
                List<AuditEntry> changes = new();
                foreach (PropertyEntry property in entityEntry.Properties)
                {
                    if ((isAdd && property.CurrentValue != null) || (property.IsModified && !Object.Equals(property.CurrentValue, property.OriginalValue)))
                    {
                        if (property.Metadata.Name != "Id") // Do not track primary key values (never going to change)
                        {
                            changes.Add(new AuditEntry()
                            {
                                FieldName = property.Metadata.Name,
                                NewValue = property.CurrentValue?.ToString(),
                                OldValue = isAdd ? null : property.OriginalValue?.ToString()
                            });
                        }
                    }
                }
                PropertyEntry? IsDeletedPropertyEntry = entityEntry.Properties.FirstOrDefault(x => x.Metadata.Name == nameof(entity.IsDeleted));
                entityAuditInformation.Add(new EntityAuditInformation()
                {
                    Entity = entity,
                    TableName = entityEntry.Metadata?.GetTableName() ?? entity.GetType().Name,
                    State = entityEntry.State,
                    IsDeleteChanged = IsDeletedPropertyEntry != null && !object.Equals(IsDeletedPropertyEntry.CurrentValue, IsDeletedPropertyEntry.OriginalValue),
                    Changes = changes
                });
            }

            return entityAuditInformation;
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entityAuditInformation = BeforeSaveChanges();
            int returnValue = 0;
            var userId = await Users.Select(x => x.Id).FirstOrDefaultAsync();
            returnValue = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            if (returnValue > 0)
            {
                foreach (EntityAuditInformation item in entityAuditInformation)
                {
                    dynamic entity = item.Entity;
                    List<AuditEntry> changes = item.Changes;
                    if (changes != null && changes.Any())
                    {
                        Audit audit = new()
                        {
                            TableName = item.TableName,
                            RecordId = entity.Id,
                            ChangeDate = DateTime.Now,
                            Operation = item.OperationType,
                            Changes = changes,
                            ChangedById = userId // LoggedIn user Id
                        };
                        _ = await AddAsync(audit, cancellationToken);
                    }
                }

                //Save audit data
                await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            return returnValue;
        }
    }
}
