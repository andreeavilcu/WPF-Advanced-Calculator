using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Input;
using CalculatorApp.ViewModels;


namespace CalculatorApp.Views
{
    public partial class MainWindow : Window
    {
        private CalculatorViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new CalculatorViewModel();
            this.DataContext = _viewModel;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool ctrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                if (!ctrlPressed) 
                {
                    string number = (e.Key >= Key.D0 && e.Key <= Key.D9)
                        ? (e.Key - Key.D0).ToString()
                        : (e.Key - Key.NumPad0).ToString();
                    _viewModel.NumberCommand.Execute(number);
                    e.Handled = true;
                }
            }
           
            else if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                _viewModel.CalculateCommand.Execute(null);
                e.Handled = true;
            }
            else if ((e.Key == Key.Add) || (e.Key == Key.OemPlus && shiftPressed))
            {
                _viewModel.OperationCommand.Execute("+");
                e.Handled = true;
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                _viewModel.OperationCommand.Execute("-");
                e.Handled = true;
            }
            else if (e.Key == Key.Multiply || (e.Key == Key.D8 && shiftPressed))
            {
                _viewModel.OperationCommand.Execute("*");
                e.Handled = true;
            }
            else if (e.Key == Key.Divide || e.Key == Key.OemQuestion || e.Key == Key.Oem2)
            {
                _viewModel.OperationCommand.Execute("/");
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                _viewModel.ClearCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Delete)
            {
                _viewModel.ClearEntryCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                _viewModel.BackspaceCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.OemPeriod || e.Key == Key.Decimal || e.Key == Key.OemComma)
            {
                _viewModel.NumberCommand.Execute(".");
                e.Handled = true;
            }
            else if (e.Key == Key.F1)
            {
                _viewModel.ShowAboutCommand.Execute(null);
                e.Handled = true;
            }

            
            if (ctrlPressed)
            {
                if (e.Key == Key.C)
                {
                    _viewModel.CopyCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.V)
                {
                    _viewModel.PasteCommand.Execute(null);
                    e.Handled = true;
                }
                else if (e.Key == Key.X)
                {
                    _viewModel.CutCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }


    }
}
