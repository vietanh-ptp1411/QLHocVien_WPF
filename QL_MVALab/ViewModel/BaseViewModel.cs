using System;
using System.Windows.Input;

namespace QL_MVALab.ViewModel
{
    // RelayCommand implementation for MVVM pattern
    public class RelayCommand<T> : ICommand
    {
        #region Fields
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;
        #endregion

        #region Constructors
        public RelayCommand(Predicate<T> canExecute, Action<T> execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute;
            _execute = execute;
        }

        public RelayCommand(Action<T> execute) : this(null, execute)
        {
        }
        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            try
            {
                return _canExecute == null || _canExecute((T)parameter);
            }
            catch
            {
                return true;
            }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion
    }

    // Generic RelayCommand without parameter
    public class RelayCommand : ICommand
    {
        #region Fields
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;
        #endregion

        #region Constructors
        public RelayCommand(Func<bool> canExecute, Action execute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _canExecute = canExecute;
            _execute = execute;
        }

        public RelayCommand(Action execute) : this(null, execute)
        {
        }
        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion
    }
}