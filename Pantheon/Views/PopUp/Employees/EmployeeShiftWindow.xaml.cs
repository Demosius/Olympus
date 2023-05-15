using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Pantheon.ViewModels.Controls.Employees;
using Pantheon.ViewModels.PopUp.Employees;

namespace Pantheon.Views.PopUp.Employees;

/// <summary>
/// Interaction logic for EmployeeShiftWindow.xaml
/// </summary>
public partial class EmployeeShiftWindow
{
    public EmployeeShiftVM? VM { get; set; }
    public EmployeeVM EmployeeVM { get; set; }

    public EmployeeShiftWindow(EmployeeVM employee)
    {
        EmployeeVM = employee;
        InitializeComponent();
    }

    private async void EmployeeShiftWindow_OnInitialized(object? sender, EventArgs e)
    {
        VM = await EmployeeShiftVM.CreateAsync(EmployeeVM);
        DataContext = VM;
    }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // Fixes issue when clicking cut/copy/paste in context menu
        if (textBox.SelectionLength == 0)
            textBox.SelectAll();
    }

    private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // If user highlights some text, don't override it
        if (textBox.SelectionLength == 0)
            textBox.SelectAll();

        // further clicks will not select all
        textBox.LostMouseCapture -= TextBox_LostMouseCapture;
    }

    private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // once we've left the TextBox, return the select all behavior
        textBox.LostMouseCapture += TextBox_LostMouseCapture;
    }
}