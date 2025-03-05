using IT_Company.Models;
using IT_Company.ViewModels;
using System;
using System.Linq;
using System.Windows;

namespace IT_Company
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                using var db = new EmployeePositionContext();
                var employees = db.Employees;
                var viewModel = new MainViewModel(employees);
                this.DataContext = viewModel;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                this.Close();
            }
        }
    }
}