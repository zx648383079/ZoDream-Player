using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZoDream.Player.ViewModels
{
    public class DelegateCommand<T> : ICommand
    {
        readonly Action<T> _execute;

        public DelegateCommand(Action<T> OnExecute)
        {
            _execute = OnExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute?.Invoke((T)parameter);
    }
}
