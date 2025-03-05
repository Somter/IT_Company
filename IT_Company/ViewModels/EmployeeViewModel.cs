using IT_Company.Models;

namespace IT_Company.ViewModels
{
    public class EmployeeViewModel : ViewModelBase
    {
        private Employee employee;
        public EmployeeViewModel(Employee e) { employee = e; }
        public int Id => employee.Id;

        public string LastName
        {
            get => employee.LastName;
            set
            {
                employee.LastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public string FirstName
        {
            get => employee.FirstName;
            set
            {
                employee.FirstName = value;
                OnPropertyChanged(nameof(FirstName)); 
            }
        }

        public string PositionTitle
        {
            get => employee.Position?.Title;
        }

        public int Age
        {
            get => employee.Age;
            set
            {
                employee.Age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        public int PositionId
        {
            get => employee.PositionId;
            set
            {
                employee.PositionId = value;
                OnPropertyChanged(nameof(PositionId));
            }
        }
    }
}