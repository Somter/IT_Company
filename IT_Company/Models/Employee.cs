namespace IT_Company.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
    }
}