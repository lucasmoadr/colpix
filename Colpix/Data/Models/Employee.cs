using System.Text.Json.Serialization;

namespace Colpix.Data.Models
{
    public class Employee
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Supervisor_id { get; set; }


        public DateTime LastUpdate { get; private set; } = DateTime.Now;

        public void InsertOrUpdate()
        {
            LastUpdate = DateTime.Now;
        }
    }
}
