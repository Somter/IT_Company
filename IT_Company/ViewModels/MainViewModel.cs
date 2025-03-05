using IT_Company.Commands;
using IT_Company.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace IT_Company.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableCollection<EmployeeDisplay> employeesList;
        public ObservableCollection<EmployeeDisplay> EmployeesList
        {
            get => employeesList;
            set
            {
                employeesList = value;
                OnPropertyChanged(nameof(EmployeesList));
            }
        }

        private string lastName;
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        private string firstName;
        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string age;
        public string Age
        {
            get => age;
            set
            {
                age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        private string positionTitle;
        public string PositionTitle
        {
            get => positionTitle;
            set
            {
                positionTitle = value;
                OnPropertyChanged(nameof(PositionTitle));
            }
        }

        private int selectedEmployeeIndex = -1;
        public int SelectedEmployeeIndex
        {
            get => selectedEmployeeIndex;
            set
            {
                selectedEmployeeIndex = value;
                OnPropertyChanged(nameof(SelectedEmployeeIndex));
                LoadSelectedEmployee();
            }
        }

        public MainViewModel(IQueryable<Employee> employees)
        {
            EmployeesList = new ObservableCollection<EmployeeDisplay>(
                employees
                .OrderBy(e => e.LastName)
                .Select(e => new EmployeeDisplay
                {
                    Id = e.Id,
                    LastName = e.LastName,
                    FirstName = e.FirstName,
                    Age = e.Age,
                    PositionTitle = e.Position.Title
                })
            );
            ClearFieldsAndSelection();
        }

        private void LoadSelectedEmployee()
        {
            if (SelectedEmployeeIndex >= 0 && SelectedEmployeeIndex < EmployeesList.Count)
            {
                var emp = EmployeesList[SelectedEmployeeIndex];
                LastName = emp.LastName;
                FirstName = emp.FirstName;
                Age = emp.Age.ToString();
                PositionTitle = emp.PositionTitle;
            }
            else
            {
                ClearFields();
            }
            CommandManager.InvalidateRequerySuggested();
        }

        private void ClearFieldsAndSelection()
        {
            SelectedEmployeeIndex = -1;
        }

        private void ClearFields()
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            Age = string.Empty;
            PositionTitle = string.Empty;
        }

        private DelegateCommand addCommand;
        public ICommand AddCommand => addCommand ??= new DelegateCommand(_ => AddEmployee(), _ => CanAddEmployee());

        private bool CanAddEmployee()
        {
            if (SelectedEmployeeIndex != -1) return false;
            return !string.IsNullOrWhiteSpace(LastName)
                && !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(Age)
                && !string.IsNullOrWhiteSpace(PositionTitle)
                && int.TryParse(Age, out int a) && a > 0;
        }

        private void AddEmployee()
        {
            try
            {
                using var db = new EmployeePositionContext();
                var title = PositionTitle.Trim();
                var pos = db.Positions.FirstOrDefault(p => p.Title == title);
                if (pos == null)
                {
                    pos = new Position { Title = title };
                    db.Positions.Add(pos);
                    db.SaveChanges();
                }
                var ageInt = int.Parse(Age);
                var newEmp = new Employee
                {
                    LastName = LastName.Trim(),
                    FirstName = FirstName.Trim(),
                    Age = ageInt,
                    PositionId = pos.Id
                };
                db.Employees.Add(newEmp);
                db.SaveChanges();
                EmployeesList.Add(new EmployeeDisplay
                {
                    Id = newEmp.Id,
                    LastName = newEmp.LastName,
                    FirstName = newEmp.FirstName,
                    Age = newEmp.Age,
                    PositionTitle = pos.Title
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            ClearFieldsAndSelection();
        }

        private DelegateCommand deleteCommand;
        public ICommand DeleteCommand => deleteCommand ??= new DelegateCommand(_ => DeleteEmployee(), _ => CanDeleteEmployee());
        private bool CanDeleteEmployee()
        {
            return (SelectedEmployeeIndex >= 0 && SelectedEmployeeIndex < EmployeesList.Count);
        }

        private void DeleteEmployee()
        {
            if (!CanDeleteEmployee()) return;
            try
            {
                var emp = EmployeesList[SelectedEmployeeIndex];
                var ask = MessageBox.Show($"Удалить сотрудника {emp.LastName} {emp.FirstName}?",
                    "Удаление", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (ask == MessageBoxResult.Cancel) return;
                using var db = new EmployeePositionContext();
                var dbEmp = db.Employees.FirstOrDefault(e => e.Id == emp.Id);
                if (dbEmp != null)
                {
                    db.Employees.Remove(dbEmp);
                    db.SaveChanges();
                    EmployeesList.RemoveAt(SelectedEmployeeIndex);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            ClearFieldsAndSelection();
        }

        private DelegateCommand updateCommand;
        public ICommand UpdateCommand => updateCommand ??= new DelegateCommand(_ => UpdateEmployee(), _ => CanUpdateEmployee());
        private bool CanUpdateEmployee()
        {
            if (SelectedEmployeeIndex < 0 || SelectedEmployeeIndex >= EmployeesList.Count)
                return false;
            return !string.IsNullOrWhiteSpace(LastName)
                && !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(Age)
                && !string.IsNullOrWhiteSpace(PositionTitle)
                && int.TryParse(Age, out int a) && a > 0;
        }

        private void UpdateEmployee()
        {
            if (!CanUpdateEmployee()) return;
            try
            {
                var empVM = EmployeesList[SelectedEmployeeIndex];
                using var db = new EmployeePositionContext();
                var dbEmp = db.Employees.FirstOrDefault(e => e.Id == empVM.Id);
                if (dbEmp == null) return;
                var title = PositionTitle.Trim();
                var pos = db.Positions.FirstOrDefault(p => p.Title == title);
                if (pos == null)
                {
                    pos = new Position { Title = title };
                    db.Positions.Add(pos);
                    db.SaveChanges();
                }
                dbEmp.LastName = LastName.Trim();
                dbEmp.FirstName = FirstName.Trim();
                dbEmp.Age = int.Parse(Age);
                dbEmp.PositionId = pos.Id;
                db.SaveChanges();
                var updatedItem = new EmployeeDisplay
                {
                    Id = dbEmp.Id,
                    LastName = dbEmp.LastName,
                    FirstName = dbEmp.FirstName,
                    Age = dbEmp.Age,
                    PositionTitle = pos.Title
                };
                EmployeesList[SelectedEmployeeIndex] = updatedItem;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            ClearFieldsAndSelection();
        }

    }

    public class EmployeeDisplay
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public int Age { get; set; }
        public string PositionTitle { get; set; }

        public override string ToString()
        {
            return $"{LastName} {FirstName}, {Age} лет, {PositionTitle}";
        }
    }
}