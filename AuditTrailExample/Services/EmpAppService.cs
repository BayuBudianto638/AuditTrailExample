using AuditTrailExample.Data;
using AuditTrailExample.Models;
using Microsoft.EntityFrameworkCore;

namespace AuditTrailExample.Services
{
    public class EmpAppService
    {
        private readonly EmpDbContext _context;
        public EmpAppService(EmpDbContext context)
        {
            _context = context;
        }
        public async Task<List<Employee>> GetAll()
        {
            return await _context.Employees.Where(x => x.IsDeleted == false).ToListAsync();
        }
        public async Task<Employee> GetById(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> Delete(int id)
        {
            var record = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (record != null)
            {
                record.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> Save(Employee model)
        {
            if (model.Id == 0)
            {
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                var record = await _context.Employees.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (record != null)
                {
                    record.Name = model.Name;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }
    }
}
