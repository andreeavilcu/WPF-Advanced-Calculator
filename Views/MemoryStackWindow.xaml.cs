using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;

namespace CalculatorApp.Views
{
    public partial class MemoryStackWindow : Window
    {
        public double? SelectedValue { get; private set; }
        public MemoryStackWindow(List<double> memoryValues)
        {
            InitializeComponent();

            this.memoryListBox.ItemsSource = memoryValues.Select(v => v.ToString());
        }

        private void UseButton_Click(object sender, RoutedEventArgs e)
        {
            if (memoryListBox.SelectedItem != null)
            {
                SelectedValue = Convert.ToDouble(memoryListBox.SelectedItem);
                DialogResult = true;
                Close();    
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
