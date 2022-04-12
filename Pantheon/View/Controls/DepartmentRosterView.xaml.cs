using System.Windows.Controls;
using System.Windows.Input;

namespace Pantheon.View.Controls;

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