using AuditTrailExample.Models;
using AuditTrailExample.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuditTrailExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly EmpAppService _employeeService;

        public EmployeeController(EmpAppService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeService.GetAll();

            if (employees == null || !employees.Any())
            {
                return NotFound();
            }

            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeService.GetById(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var success = await _employeeService.Delete(id);
            if (!success)
            {
                return NotFound();
            }
            
            return NoContent(); 
        }

        [HttpPost]
        public async Task<IActionResult> SaveEmployee(Employee model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var success = await _employeeService.Save(model);
            if (!success)
            {
                return StatusCode(500, "Error saving employee"); 
            }

            return CreatedAtRoute("GetEmployeeById", new { id = model.Id }, model); 
        }
    }
}
