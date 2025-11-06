
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Colpix.Data.Models;
using Colpix.Data.Repositories;

namespace Colpix.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.Employees.ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var empCargo = _db.Employees.Where(x=>x.Supervisor_id==id).Count();
            return await _db.Employees.FindAsync(id);
        }

        public async Task<Employee> InsertAsync(Employee employee)
        {
            employee.InsertOrUpdate();
            await _db.Employees.AddAsync(employee);
            await _db.SaveChangesAsync();
            return employee;
        }

        public async Task<EmployeeDetailsDto?> GetDetailsByIdAsync(int id)
        {
            // Fire both DB tasks in parallel
            var employeeTask = _db.Employees.FindAsync(id).AsTask();
            var allEmployeesTask = _db.Employees.ToListAsync();

            await Task.WhenAll(employeeTask, allEmployeesTask);

            var employee = await employeeTask;
            if (employee == null) return null;

            var all = await allEmployeesTask;

            // Construir lookup supervisor -> lista de ids para recorrido eficiente
            var lookup = all
                .GroupBy(e => e.Supervisor_id)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToList());

            // Contar reportes directos e indirectos con DFS (stack)
            var stack = new Stack<int>();
            if (lookup.TryGetValue(employee.Id, out var directs))
            {
                foreach (var d in directs) stack.Push(d);
            }

            var count = 0;
            while (stack.Count > 0)
            {
                var currentId = stack.Pop();
                count++;

                if (lookup.TryGetValue(currentId, out var children))
                {
                    foreach (var c in children) stack.Push(c);
                }
            }

            return new EmployeeDetailsDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Supervisor_id = employee.Supervisor_id,
                LastUpdate = employee.LastUpdate,
                ReportsCount = count
            };
        }

        public async Task<Employee> Update(int idEmpleado, Employee employee)
        {
            var existingEmployee = await _db.Employees.FindAsync(idEmpleado);
            if (existingEmployee == null) return null;
            employee.InsertOrUpdate();
            existingEmployee.Email= employee.Email;
            existingEmployee.Name= employee.Name;
            existingEmployee.Supervisor_id= employee.Supervisor_id;
            
            await _db.SaveChangesAsync();
            return employee;
        }


    }
}