using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colpix.Data.Models;
using Colpix.Services;
using Microsoft.AspNetCore.Authorization;

namespace Colpix.Controllers
{
    /// <summary>
    /// Controlador para la gestión de empleados.
    /// Requiere un token JWT válido (Bearer) para todas las operaciones.
    /// </summary>
    /// <remarks>
    /// 🔐 **Autenticación:**  
    /// Todos los endpoints de este controlador requieren un token JWT válido en el encabezado `Authorization`.  
    /// Formato esperado:
    /// 
    /// Authorization: Bearer {tu_token_jwt}
    /// 
    /// - **401 Unauthorized** → El token es inválido, expiró o no se envió.  
    /// - **403 Forbidden** → El token es válido pero el usuario no tiene permisos suficientes.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService service)
        {
            _employeeService = service;
        }

        /// <summary>
        /// Obtiene todos los empleados registrados.
        /// </summary>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     GET /Employee
        ///
        /// Ejemplo de respuesta:
        ///
        ///     [
        ///       { "id": 1, "name": "Lucas Olivella", "email": "lucas@empresa.com", "supervisor_id": 0 },
        ///       { "id": 2, "name": "Ana Pérez", "email": "ana@empresa.com", "supervisor_id": 1 }
        ///     ]
        /// </remarks>
        /// <response code="200">Lista de empleados obtenida correctamente.</response>
        /// <response code="401">Token JWT inválido o no enviado.</response>
        /// <response code="403">El usuario no tiene permisos para acceder.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Employee>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Employee>> Get()
        {
            return await _employeeService.GetAllAsync();
        }

        /// <summary>
        /// Obtiene los detalles de un empleado por su ID, incluyendo cantidad de reportes.
        /// </summary>
        /// <param name="id">Identificador único del empleado.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     GET /Employee/5
        ///
        /// Ejemplo de respuesta:
        ///
        ///     {
        ///       "id": 5,
        ///       "name": "Lucas Olivella",
        ///       "email": "lucas@empresa.com",
        ///       "supervisor_id": 1,
        ///       "lastUpdate": "2025-11-05T10:45:00Z",
        ///       "reportsCount": 4
        ///     }
        /// </remarks>
        /// <response code="200">Devuelve los detalles del empleado solicitado.</response>
        /// <response code="401">Token JWT inválido o no enviado.</response>
        /// <response code="403">El usuario no tiene permisos para acceder.</response>
        /// <response code="404">Si no se encuentra el empleado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeDetailsDto), StatusCodes.Status200OK)]
      /*  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]*/
        public async Task<IActionResult> GetById(int id)
        {
            var dto = await _employeeService.GetDetailsByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// Crea un nuevo empleado en la base de datos.
        /// </summary>
        /// <param name="employee">Objeto empleado con los datos a registrar.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     POST /Employee
        ///     {
        ///       "name": "Carlos López",
        ///       "email": "carlos@empresa.com",
        ///       "supervisor_id": 1
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Empleado creado correctamente.</response>
        /// <response code="400">Si los datos del empleado son inválidos.</response>
        /// <response code="401">Token JWT inválido o no enviado.</response>
        /// <response code="403">El usuario no tiene permisos para crear empleados.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
       
        public async Task<IActionResult> Insert(Employee employee)
        {
            var created = await _employeeService.InsertAsync(employee);
            return Ok("Empleado creado correctamente.");
        }

        /// <summary>
        /// Modifica un empleado existente.
        /// </summary>
        /// <param name="idEmpleado">ID del empleado a modificar.</param>
        /// <param name="employee">Datos actualizados del empleado.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        ///
        ///     PUT /Employee?idEmpleado=3
        ///     {
        ///       "name": "Carlos López",
        ///       "email": "carlos.lopez@empresa.com",
        ///       "supervisor_id": 1
        ///     }
        /// </remarks>
        /// <response code="200">Empleado modificado correctamente.</response>
        /// <response code="401">Token JWT inválido o no enviado.</response>
        /// <response code="403">El usuario no tiene permisos para modificar empleados.</response>
        /// <response code="404">Si el empleado no existe.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
       
        public async Task<ActionResult> Edit(int idEmpleado, Employee employee)
        {
            var emp = await _employeeService.Update(idEmpleado, employee);
            if (emp == null)
                return NotFound();

            return Ok($"Empleado {idEmpleado} modificado correctamente.");
        }
    }
}
