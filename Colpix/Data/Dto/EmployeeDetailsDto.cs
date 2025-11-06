using System;

namespace Colpix.Data.Models
{
    // DTO para el detalle solicitado: atributos del empleado + LastUpdate + cantidad de reportes (directos e indirectos)
    public class EmployeeDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Supervisor_id { get; set; }
        public DateTime LastUpdate { get; set; }
        public int ReportsCount { get; set; }
    }
}