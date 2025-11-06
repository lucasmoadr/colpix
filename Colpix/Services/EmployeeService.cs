
using Colpix.Data.Models;
using Colpix.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colpix.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public EmployeeService(AppDbContext db, IDbContextFactory<AppDbContext> dbFactory)
        {
            _db = db;
            _dbFactory = dbFactory;

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
        #region metodo antiguo
        //public async Task<EmployeeDetailsDto?> GetDetailsByIdAsync1(int id)
        //{
        //    // Fire both DB tasks in parallel
        //    var employeeTask = _db.Employees.FindAsync(id).AsTask();
        //    var allEmployeesTask = _db.Employees.ToListAsync();

        //    await Task.WhenAll(employeeTask, allEmployeesTask);

        //    var employee = await employeeTask;
        //    if (employee == null) return null;

        //    var all = await allEmployeesTask;

        //    // Construir lookup supervisor -> lista de ids para recorrido eficiente
        //    var lookup = all
        //        .GroupBy(e => e.Supervisor_id)
        //        .ToDictionary(g => g.Key, g => g.Select(e => e.Id).ToList());

        //    // Contar reportes directos e indirectos con DFS (stack)
        //    var stack = new Stack<int>();
        //    if (lookup.TryGetValue(employee.Id, out var directs))
        //    {
        //        foreach (var d in directs) stack.Push(d);
        //    }

        //    var count = 0;
        //    while (stack.Count > 0)
        //    {
        //        var currentId = stack.Pop();
        //        count++;

        //        if (lookup.TryGetValue(currentId, out var children))
        //        {
        //            foreach (var c in children) stack.Push(c);
        //        }
        //    }

        //    return new EmployeeDetailsDto
        //    {
        //        Id = employee.Id,
        //        Name = employee.Name,
        //        Email = employee.Email,
        //        Supervisor_id = employee.Supervisor_id,
        //        LastUpdate = employee.LastUpdate,
        //        ReportsCount = count
        //    };
        //}
        #endregion
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



        public async Task<EmployeeDetailsDto?> GetDetailsByIdAsync(int id)
        {
            // Obtener el empleado (una sola llamada)
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null) return null;

            // obtener ids de reportes directos (una llamada)
            var directIds = await _db.Employees
                .Where(e => e.Supervisor_id == id)
                .Select(e => e.Id)
                .ToListAsync();

            if (directIds.Count == 0)
            {
                return new EmployeeDetailsDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Supervisor_id = employee.Supervisor_id,
                    LastUpdate = employee.LastUpdate,
                    ReportsCount = 0
                };
            }

            // Visited concurrent para evitar duplicados/ciclos al contar en paralelo
            var visited = new ConcurrentDictionary<int, byte>();

            // Para cada reporte directo arrancamos una tarea que cuenta su subárbol.
            var tasks = new List<Task<int>>(directIds.Count);

            foreach (var rootId in directIds)
            {
                // Intentar marcar root como visitado; si ya estaba, lo omitimos.
                if (!visited.TryAdd(rootId, 0)) continue;

                tasks.Add(CountSubtreeAsync(rootId, visited));
            }

            // Ejecutar todas las tareas en paralelo 
            var results = await Task.WhenAll(tasks);
            var reportsCount = results.Sum();

            return new EmployeeDetailsDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Supervisor_id = employee.Supervisor_id,
                LastUpdate = employee.LastUpdate,
                ReportsCount = reportsCount
            };

           
            async Task<int> CountSubtreeAsync(int rootId, ConcurrentDictionary<int, byte> sharedVisited)
            {
                await using var ctx = _dbFactory.CreateDbContext();

                var count = 1; 
                var queue = new Queue<int>();
                queue.Enqueue(rootId);


                while (queue.Count > 0)
                {
                    var IdList = new List<int>();
                    while (queue.Count > 0)
                        IdList.Add(queue.Dequeue());

                    // Obtener hijos de todos los ids del batch en una sola consulta
                    var children = await ctx.Employees
                        .Where(e => IdList.Contains(e.Supervisor_id))
                        .Select(e => e.Id)
                        .ToListAsync();

                    foreach (var childId in children)
                    {
                        //  solo los nuevos se cuentan
                        if (sharedVisited.TryAdd(childId, 0))
                        {
                            count++;
                            queue.Enqueue(childId);
                        }
                    }
                }

                return count;
            }
        }





    }
}