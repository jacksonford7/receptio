using System;
using System.Diagnostics;
using System.Windows.Input;

namespace RECEPTIO.CapaPresentacion.UI.MVVM
{
    public class RelayCommand<T> : ICommand
    {
        #region Declarations
        readonly Func<Boolean> _canExecute;
        readonly Action _execute;
        #endregion

        #region Constructors
        public RelayCommand(Action execute, Func<Boolean> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute();
        }
        #endregion
    }
    public class RelayCommand : ICommand
    {
        #region Declarations
        readonly Func<Boolean> _canExecute;
        readonly Action _execute;
        #endregion

        #region Constructors
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
        public event EventHandler CanExecuteChanged
        {
            add
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {

                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(Object parameter)
        {
            _execute();
        }
        #endregion
    }
}
