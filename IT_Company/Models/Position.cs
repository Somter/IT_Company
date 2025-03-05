using System.Collections.Generic;

namespace IT_Company.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = new HashSet<Employee>();
    }
}