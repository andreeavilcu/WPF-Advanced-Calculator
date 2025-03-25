using CalculatorApp.Helpers;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using CalculatorApp.Views;
using System.Collections.Generic;

namespace CalculatorApp.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private string _displayText = "0";
        private double _currentValue = 0;
        private string _currentOperation = "";
        private bool _isNewInput = true;
        private string _firstNumber = "";
        private string _secondNumber = "";

        private bool _isProgrammerMode = false;
        private int _currentBase = 10;
        private int _previousBase = 10;
        private bool _isDigitGroupingEnabled = false;
        private bool _respectOperatorPrecedence = false;

        private string _hexValue = "0";
        private string _decValue = "0";
        private string _octValue = "0";
        private string _binValue = "0";

        private string _clipboardText = "";
        private double _memoryValue = 0;
        private List<double> _memoryStack = new List<double>();


        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get
            {
                if (IsDigitGroupingEnabled && (!_isProgrammerMode || CurrentBase == 10) && _displayText != "Error")
                {
                    if (double.TryParse(_displayText, out double value))
                    {
                        return FormatWithDigitGrouping(value);
                    }
                }
                return _displayText;
            }
            set
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }

        public string FirstNumber
        {
            get => _firstNumber;
            set => _firstNumber = value;
        }

        public string SecondNumber
        {
            get => _secondNumber;
            set => _secondNumber = value;
        }

        public bool IsProgrammerMode
        {
            get => _isProgrammerMode;
            set
            {
                _isProgrammerMode = value;
                OnPropertyChanged(nameof(IsProgrammerMode));
                if (value)
                {
                    CurrentBase = 10;
                }
                SaveSettings();
            }
        }

        public int CurrentBase
        {
            get => _currentBase;
            set
            {
                _previousBase = _currentBase;
                _currentBase = value;
                OnPropertyChanged(nameof(CurrentBase));
                if (!string.IsNullOrEmpty(DisplayText) && DisplayText != "Error")
                {
                    UpdateDisplayForBase(_previousBase, _currentBase);
                }
                SaveSettings();
            }

        }

        public bool IsDigitGroupingEnabled
        {
            get => _isDigitGroupingEnabled;
            set
            {
                _isDigitGroupingEnabled = value;
                OnPropertyChanged(nameof(IsDigitGroupingEnabled));
                _displayText = RemoveDigitGrouping(_displayText); 
                DisplayText = _displayText;
                UpdateDisplayWithDigitGrouping();
                SaveSettings(); 
            }
        }

        public string HexValue
        {
            get => _hexValue;
            set
            {
                _hexValue = value;
                OnPropertyChanged(nameof(HexValue));
            }
        }


        public string DecValue
        {
            get => _decValue;
            set
            {
                _decValue = value;
                OnPropertyChanged(nameof(DecValue));
            }
        }

        public string OctValue
        {
            get => _octValue;
            set
            {
                _octValue = value;
                OnPropertyChanged(nameof(OctValue));
            }
        }

        public string BinValue
        {
            get => _binValue;
            set
            {
                _binValue = value;
                OnPropertyChanged(nameof(BinValue));
            }
        }

        private String ClipboardText
        {
            get => _clipboardText;
            set => _clipboardText = value;
        }


        public ICommand NumberCommand { get; private set;  }
        public ICommand OperationCommand { get; private set; }
        public ICommand CalculateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand ClearEntryCommand { get; private set; }
        public ICommand BackspaceCommand { get; private set; }
        public ICommand SpecialOperationCommand { get; private set; }
        public ICommand MemoryClearCommand { get; private set; }
        public ICommand MemoryRecallCommand { get; private set; }
        public ICommand MemoryAddCommand { get; private set; }
        public ICommand MemorySubstractCommand { get; private set; }
        public ICommand MemoryStoreCommand { get; private set; }
        public ICommand ShowMemoryStackCommand { get; private set; }
        public ICommand SetBaseCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand ShowAboutCommand { get; private set; }

        public CalculatorViewModel()
        {
            LoadSettings();
            InitializeCommands();
            UpdateAllBaseValues();
        }

        private void InitializeCommands()
        {
            NumberCommand = new RelayCommand(param => EnterNumber(param.ToString()));
            OperationCommand = new RelayCommand(param => SetOperation(param.ToString()));
            CalculateCommand = new RelayCommand(_ => Calculate());
            ClearCommand = new RelayCommand(_ => ClearAll());
            ClearEntryCommand = new RelayCommand(_ => ClearEntry());
            BackspaceCommand = new RelayCommand(_ => Backspace());
            SpecialOperationCommand = new RelayCommand(param => PerformSpecialOperation(param.ToString()));
            SetBaseCommand = new RelayCommand(param => SetBase(int.Parse(param.ToString())));

            MemoryClearCommand = new RelayCommand(_ => MemoryClear());
            MemoryRecallCommand = new RelayCommand(_ => MemoryRecall());
            MemoryAddCommand = new RelayCommand(_ => MemoryAdd());
            MemorySubstractCommand = new RelayCommand(_ => MemorySubstract());
            MemoryStoreCommand = new RelayCommand(_ => MemoryStore());
            ShowMemoryStackCommand = new RelayCommand(_ => ShowMemoryStack());

           
            CutCommand = new RelayCommand(_ => Cut());
            CopyCommand = new RelayCommand(_ => Copy());
            PasteCommand = new RelayCommand(_ => Paste());
            ShowAboutCommand = new RelayCommand(_ => ShowAbout());
        }

        private double GetValueFromDisplay()
        {
            try
            {
                if (_isProgrammerMode && CurrentBase != 10)
                {
                    return Convert.ToInt64(DisplayText, CurrentBase);
                }
                return double.Parse(DisplayText);
            }
            catch
            {
                DisplayText = "Error";
                ClearAll(); 
                return 0;
            }
        }
        private void Negate()
        {
            if (DisplayText == "0" || DisplayText == "Error")
                return;

            try
            {
                long value;
                switch (CurrentBase)
                {
                    case 2:
                        value = Convert.ToInt64(DisplayText, 2);
                        value = ~value + 1; 
                        DisplayText = Convert.ToString(value, 2);
                        break;
                    case 8:
                        value = Convert.ToInt64(DisplayText, 8);
                        value = ~value + 1;
                        DisplayText = Convert.ToString(value, 8);
                        break;
                    case 10:
                        double decimalValue = double.Parse(DisplayText);
                        decimalValue = -decimalValue;
                        DisplayText = decimalValue.ToString();
                        break;
                    case 16:
                        value = Convert.ToInt64(DisplayText, 16);
                        value = ~value + 1;
                        DisplayText = Convert.ToString(value, 16).ToUpper();
                        break;
                }
            }
            catch
            {
                DisplayText = "Error";
            }

            UpdateAllBaseValues();
        }
        private void EnterNumber(string number)
        {
            if (IsProgrammerMode)
            {
                if (CurrentBase == 16)
                {
                    if (!IsValidHexDigit(number, CurrentBase))
                        return;
                }
                else if (!isValidDigitForBase(number, CurrentBase))
                {
                    return;
                }

                if (number == "." && CurrentBase != 10)
                    return;

                UpdateAllBaseValues();
            }

            if (number == "." && DisplayText.Contains("."))
                return;
 
            if (_isNewInput)
            {
                DisplayText = number;
                _isNewInput = false;
            }
            else
            {
                DisplayText += number;
            }

            if (string.IsNullOrEmpty(_currentOperation))
            {
                FirstNumber = DisplayText;
            }
            else
            {
                SecondNumber = DisplayText;
            }
        }
        private void SetOperation(string operation)
        {
            if (operation == "Negate")
            {
                Negate();
                return;
            }

            if (string.IsNullOrEmpty(DisplayText))
                return;

            try
            {
                FirstNumber = DisplayText;

                if (IsProgrammerMode)
                {
                    switch (CurrentBase)
                    {
                        case 2:
                            _currentValue = Convert.ToInt64(DisplayText, 2);
                            break;
                        case 8:
                            _currentValue = Convert.ToInt64(DisplayText, 8);
                            break;
                        case 16:
                            _currentValue = Convert.ToInt64(DisplayText, 16);
                            break;
                        default:
                            _currentValue = double.Parse(DisplayText);
                            break;
                    }
                }
                else
                {
                    _currentValue = double.Parse(DisplayText);
                }
            }
            catch
            {
                DisplayText = "Error";
                return;
            }

            _currentOperation = operation;
            _isNewInput = true;

            DisplayText = FirstNumber + " " + operation + " ";
            UpdateAllBaseValues();
        }
        private void Calculate()
        {
            if (string.IsNullOrEmpty(_currentOperation))
                return;

            PerformCalculation();
            UpdateAllBaseValues();
        }
        private void PerformCalculation()
        {
            if (string.IsNullOrEmpty(SecondNumber))
                return;

            try
            {
                double result = 0;
                double num1, num2;

                if (IsProgrammerMode)
                {
                    switch (CurrentBase)
                    {
                        case 2:
                            num1 = Convert.ToInt64(FirstNumber, 2);
                            num2 = Convert.ToInt64(SecondNumber, 2);
                            break;
                        case 8:
                            num1 = Convert.ToInt64(FirstNumber, 8);
                            num2 = Convert.ToInt64(SecondNumber, 8);
                            break;
                        case 16:
                            num1 = Convert.ToInt64(FirstNumber, 16);
                            num2 = Convert.ToInt64(SecondNumber, 16);
                            break;
                        default: 
                            num1 = double.Parse(FirstNumber);
                            num2 = double.Parse(SecondNumber);
                            break;
                    }
                }
                else
                {
                    num1 = double.Parse(FirstNumber);
                    num2 = double.Parse(SecondNumber);
                }

                switch (_currentOperation)
                {
                    case "+": result = num1 + num2; break;
                    case "-": result = num1 - num2; break;
                    case "*": result = num1 * num2; break;
                    case "/":
                        if (num2 == 0)
                        {
                            DisplayText = "Error";
                            ClearAll(); 
                            return;
                        }
                        result = num1 / num2;
                        break;
                }

                if (IsProgrammerMode)
                {
                    long longResult = (long)result;
                    switch (CurrentBase)
                    {
                        case 2:
                            DisplayText = Convert.ToString(longResult, 2);
                            break;
                        case 8:
                            DisplayText = Convert.ToString(longResult, 8);
                            break;
                        case 10:
                            DisplayText = result.ToString();
                            break;
                        case 16:
                            DisplayText = Convert.ToString(longResult, 16).ToUpper();
                            break;
                    }
                }
                else
                {
                    DisplayText = result.ToString();
                }

                FirstNumber = DisplayText;
                SecondNumber = "";
                _currentOperation = "";
                _isNewInput = true;
                UpdateAllBaseValues();
            }
            catch
            {
                DisplayText = "Error";
                ClearAll(); 
            }
        }
        private void PerformSpecialOperation(string operation)
        {
            double value = GetValueFromDisplay();

            switch (operation)
            {
                case "sqrt":
                    if (value < 0)
                    {
                        DisplayText = "Error";
                        return;
                    }
                    value = Math.Sqrt(value);
                    break;

                case "square":
                    value = value * value;
                    break;

                case "inverse":
                    if (value == 0)
                    {
                        DisplayText = "Error";
                        return;
                    }
                    value = 1 / value;
                    break;

                case "percent":
                    if (!string.IsNullOrEmpty(_currentOperation) && !string.IsNullOrEmpty(SecondNumber))
                    {
                        value = _currentValue * (value / 100);
                    }
                    else
                    {
                        value = value / 100; 
                    }
                    break;
            }

            
            if (IsProgrammerMode && CurrentBase != 10)
            {
                long longResult = (long)value;
                switch (CurrentBase)
                {
                    case 2:
                        DisplayText = Convert.ToString(longResult, 2);
                        break;
                    case 8:
                        DisplayText = Convert.ToString(longResult, 8);
                        break;
                    case 16:
                        DisplayText = Convert.ToString(longResult, 16).ToUpper();
                        break;
                }
            }
            else
            {
                DisplayText = value.ToString();
            }

            _isNewInput = true;
            FirstNumber = DisplayText;  
            SecondNumber = "";           
            _currentOperation = "";    

            UpdateAllBaseValues();
        }
        private void ClearAll()
        {
            DisplayText = "0";
            SecondNumber = "";
            FirstNumber = "";
            _currentValue = 0;
            _currentOperation = "";
            _isNewInput = true;
            UpdateAllBaseValues();
        }
        private void ClearEntry()
        {
            DisplayText = "0";
            _isNewInput = true;

            if (string.IsNullOrEmpty(_currentOperation))
            {
                FirstNumber = "0";
            }
            else
            {
                SecondNumber = "0";
            }
            UpdateAllBaseValues();
        }
        private void Backspace()
        {
            if (_displayText == "0" || _displayText == "Error")
                return;

            string unformattedText = RemoveDigitGrouping(_displayText);

           
            _displayText = unformattedText.Length > 1 ?
                           unformattedText.Substring(0, unformattedText.Length - 1) :
                           "0";

            
            DisplayText = _displayText;

            if (_displayText == "0")
                _isNewInput = true;

            UpdateAllBaseValues();
        }
        private void MemoryClear()
        {
            _memoryValue = 0;
            _memoryStack.Clear();
        }
        private void MemoryRecall()
        {
            if (IsProgrammerMode && CurrentBase != 10)
            {
                long longResult = (long)_memoryValue;
                switch (CurrentBase)
                {
                    case 2:
                        DisplayText = Convert.ToString(longResult, 2);
                        break;
                    case 8:
                        DisplayText = Convert.ToString(longResult, 8);
                        break;
                    case 16:
                        DisplayText = Convert.ToString(longResult, 16).ToUpper();
                        break;
                }
            }
            else
            {
                DisplayText = _memoryValue.ToString();
            }
            _isNewInput = true;
            UpdateAllBaseValues();
        }
        private void MemoryAdd()
        {
            double currentValue = GetValueFromDisplay();
            _memoryValue += currentValue;

            //if (!_memoryStack.Contains(_memoryValue))
            //{
            //    _memoryStack.Add(_memoryValue);
            //}

            
            if (IsProgrammerMode && CurrentBase != 10)
            {
                UpdateDisplayForBase(CurrentBase, 10);
            }
            else
            {
                DisplayText = _memoryValue.ToString();
            }
            _isNewInput = true;
            UpdateAllBaseValues();
        }
        private void MemorySubstract()
        {
            double currentValue = GetValueFromDisplay();
            _memoryValue -= currentValue;

            //if (!_memoryStack.Contains(_memoryValue))
            //{
            //    _memoryStack.Add(_memoryValue);
            //}

            
            if (IsProgrammerMode && CurrentBase != 10)
            {
                UpdateDisplayForBase(CurrentBase, 10);
            }
            else
            {
                DisplayText = _memoryValue.ToString();
            }
            _isNewInput = true;
            UpdateAllBaseValues();
        }
        private void MemoryStore()
        {
            _memoryValue = GetValueFromDisplay();
            if (!_memoryStack.Contains(_memoryValue))
            {
                _memoryStack.Add(_memoryValue);
            }
        }
        private void ShowMemoryStack()
        {
            if (_memoryStack.Count == 0 && _memoryValue == 0)
                return;

            if (_memoryValue != 0 && !_memoryStack.Contains(_memoryValue))
            {
                _memoryStack.Add(_memoryValue);
            }

            var memoryWindow = new MemoryStackWindow(_memoryStack)
            {
                Owner = Application.Current.MainWindow
            };

            if (memoryWindow.ShowDialog() == true && memoryWindow.SelectedValue.HasValue)
            {
                double selectedValue = memoryWindow.SelectedValue.Value;

                if (IsProgrammerMode && CurrentBase != 10)
                {
                    long longResult = (long)selectedValue;
                    switch (CurrentBase)
                    {
                        case 2:
                            DisplayText = Convert.ToString(longResult, 2);
                            break;

                        case 8:
                            DisplayText = Convert.ToString(longResult, 8);
                            break;

                        case 16:
                            DisplayText = Convert.ToString(longResult, 16).ToUpper();
                            break;
                    }
                }
                else
                {
                    DisplayText = selectedValue.ToString();
                }
                _isNewInput = true;
            }
        }
        private void SetBase(int numBase)
        {
            _currentBase = numBase;
            UpdateDisplayForBase(_previousBase, _currentBase);
        }
        private void Cut()
        {
            Copy();
            DisplayText = "0";
            _isNewInput = true;
        }
        private void Copy()
        {
            if (DisplayText != "Error")
            {
                ClipboardText = DisplayText;
                System.Windows.Clipboard.SetText(DisplayText);
            }
        }
        private void Paste()
        {
            try
            {
                string text = System.Windows.Clipboard.GetText();
                if (double.TryParse(text, out double value))
                {
                    if (_isNewInput)
                    {
                        DisplayText = text;
                        _isNewInput = false;
                    }
                    else
                    {
                        DisplayText += text;
                    }
                }

                UpdateAllBaseValues();
            }
            catch
            {
                
            }
        }
        private void ShowAbout()
        {
            var aboutWindow = new AboutWindow
            {
                Owner = Application.Current.MainWindow
            };
            aboutWindow.ShowDialog();
        }
        private void LoadSettings()
        {
            var settings = Settings.Load();
            IsDigitGroupingEnabled = settings.IsDigitGroupingEnabled;
            IsProgrammerMode = settings.IsProgrammerMode;
            CurrentBase = settings.CurrentBase;
        }
        private void SaveSettings()
        {
            var settings = new Settings
            {
                IsDigitGroupingEnabled = IsDigitGroupingEnabled,
                IsProgrammerMode = IsProgrammerMode,
                CurrentBase = CurrentBase,

            };
            settings.Save();

        }
        private void UpdateAllBaseValues()
        {
            if (DisplayText == "Error")
            {
                HexValue = OctValue = DecValue = BinValue = "Error";
                return;
            }

            try
            {
                long decimalValue;
                if (CurrentBase == 10)
                {
                    decimalValue = (long)double.Parse(DisplayText);
                }
                else if (CurrentBase == 16)
                {
                    decimalValue = Convert.ToInt64(DisplayText, 16);
                }
                else if (CurrentBase == 8)
                {
                    decimalValue = Convert.ToInt64(DisplayText, 8);
                }
                else
                {
                    decimalValue = Convert.ToInt64(DisplayText, 2);
                }

                HexValue = Convert.ToString(decimalValue, 16).ToUpper();
                DecValue = decimalValue.ToString();
                OctValue = Convert.ToString(decimalValue, 8);
                BinValue = Convert.ToString(decimalValue, 2);
            }

            catch
            {
                HexValue = OctValue = DecValue = BinValue = "Error";
            }
        }
        private void UpdateDisplayForBase(int fromBase, int toBase)
        {
            try
            {
                long decimalValue;
                if (_displayText == "Error" || string.IsNullOrEmpty(_displayText))
                {
                    DisplayText = "0";
                    return;
                }
                string cleanDisplay = RemoveDigitGrouping(_displayText);

                if (fromBase == 10)
                {
                    if (double.TryParse(cleanDisplay, out double dValue))
                    {
                        decimalValue = (long)dValue;
                    }
                    else
                    {
                        throw new FormatException("Invalid decimal number");
                    }
                }
                else
                {
                    decimalValue = Convert.ToInt64(cleanDisplay, fromBase);
                }

                switch (toBase)
                {
                    case 2:
                        DisplayText = Convert.ToString(decimalValue, 2);
                        break;
                    case 8:
                        DisplayText = Convert.ToString(decimalValue, 8);
                        break;
                    case 10:
                        DisplayText = decimalValue.ToString();
                        break;
                    case 16:
                        DisplayText = Convert.ToString(decimalValue, 16).ToUpper();
                        break;
                }
            }
            catch
            {
                DisplayText = "Error";
            }


        }
        
        private void UpdateDisplayWithDigitGrouping()
        {
            if (!IsDigitGroupingEnabled || DisplayText == "Error")
                return;

            if (!_isProgrammerMode || CurrentBase == 10)
            {
                if (double.TryParse(DisplayText, out double value))
                {
                    DisplayText = FormatWithDigitGrouping(value);
                }
            }
        }
        private string FormatWithDigitGrouping(double value)
        {
            var culture = CultureInfo.CurrentCulture;
            var numberFormat = culture.NumberFormat;

            if (value == Math.Floor(value))
            {
                return value.ToString("N0", culture); 
            }

            
            string formattedValue = value.ToString("N", culture);

            return formattedValue;
        }
        private string RemoveDigitGrouping(string value)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out double num))
            {
                return num.ToString(CultureInfo.InvariantCulture);  
            }
            return value;
        }   
        private bool isValidDigitForBase(string digit, int currentBase)
        {
            if (digit == ".")
                return currentBase == 10; 

            if (currentBase <= 10)
            {
                return int.TryParse(digit, out int digitValue) && digitValue < currentBase;
            }

            return IsValidHexDigit(digit, currentBase);
        }
        private bool IsValidHexDigit(string digit, int currentBase)
        {
            
             return "0123456789ABCDEFabcdef".Contains(digit);
           
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}