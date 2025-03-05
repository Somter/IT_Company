using System;
using System.Windows.Input;

namespace IT_Company.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> executeAction;
        private readonly Predicate<object> canExecutePredicate;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            executeAction = execute ?? throw new ArgumentNullException(nameof(execute));
            canExecutePredicate = canExecute;
        }

        public bool CanExecute(object parameter) { return canExecutePredicate == null || canExecutePredicate(parameter); }
        public void Execute(object parameter) { executeAction(parameter); }
    }
}