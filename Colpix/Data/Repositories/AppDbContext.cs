using Colpix.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Colpix.Data.Repositories
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
       
        //    modelBuilder.Entity<Employee>().HasData(
        //        new Employee { Name = "Empleado_1", Email = "Empleado_1@colpix.com", Supervisor_id = 0 },
        //        new Employee { Name = "Empleado_2", Email = "Empleado_2@colpix.com", Supervisor_id = 1 },
        //        new Employee { Name = "Empleado_3", Email = "Empleado_3@colpix.com", Supervisor_id = 1 },
        //        new Employee { Name = "Empleado_4", Email = "Empleado_4@colpix.com", Supervisor_id = 1 },
        //        new Employee { Name = "Empleado_5", Email = "Empleado_5@colpix.com", Supervisor_id = 2 },
        //        new Employee { Name = "Empleado_6", Email = "Empleado_6@colpix.com", Supervisor_id = 3 },
        //        new Employee { Name = "Empleado_7", Email = "Empleado_7@colpix.com", Supervisor_id = 3 },
        //        new Employee { Name = "Empleado_8", Email = "Empleado_8@colpix.com", Supervisor_id = 3 },
        //        new Employee { Name = "Empleado_9", Email = "Empleado_9@colpix.com", Supervisor_id = 3 }

        //    );
        //}
    }
}