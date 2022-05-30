using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pantheon.Views.Controls;

/// <summary>
/// Interaction logic for DepartmentRosterView.xaml
/// </summary>
public partial class DepartmentRosterView
{
    public DepartmentRosterView()
    {
        InitializeComponent();
    }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        // Fixes issue when clicking cut/copy/paste in context menu
        if (textBox.SelectionLength == 0)
            textBox.SelectAll();
        if (int.TryParse(textBox.Text, out _)) return;
        textBox.Text = "0";
        e.Handled = true;
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

    private void ShiftTarget_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowed(e.Text);
    }

    private static readonly Regex regex = new("[^0-9.-]+"); //regex that matches disallowed text
    private static bool IsTextAllowed(string text)
    {
        return !regex.IsMatch(text);
    }

    private void ShiftTarget_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox textBox) return;
        if (int.TryParse(textBox.Text, out _)) return;
        textBox.Text = "0";
        e.Handled = true;
    }

    private void ShiftTarget_Changed(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (textBox.Text is "")
        {
            textBox.Text = "0";
            return;
        }

        if (int.TryParse(textBox.Text, out _)) return;

        if (textBox.Text.Length <= 9) return;

        textBox.Text = "999999999";
    }
}