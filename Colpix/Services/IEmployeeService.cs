using System.Collections.Generic;
using System.Threading.Tasks;
using Colpix.Data.Models;

namespace Colpix.Services
{
    public interface IEmployeeService
    {
        Task<Employee> Update(int idEmpleado, Employee employee);
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> InsertAsync(Employee employee);
        Task<EmployeeDetailsDto?> GetDetailsByIdAsync(int id);
    }
}