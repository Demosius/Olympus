using System;
using System.Windows.Controls;
using System.Windows.Input;
using Khaos.ViewModels;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Khaos.Views;

/// <summary>
/// Interaction logic for KhaosPage.xaml
/// </summary>
public partial class KhaosPage : IProject
{
    public KhaosPage(Helios helios)
    {
        InitializeComponent();
        DataContext = new KhaosVM(helios);
    }

    public EProject Project => EProject.Khaos;

    public static bool RequiresUser => false;

    public void RefreshData()
    {
        throw new NotImplementedException();
    }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        // Fixes issue when clicking cut/copy/paste in context menu
        if (sender is TextBox {SelectionLength: 0} textBox)
            textBox.SelectAll();
    }

    private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
    {
        var textBox = sender as TextBox;
        // If user highlights some text, don't override it
        if (textBox is {SelectionLength: 0})
            textBox.SelectAll();

        // further clicks will not select all
        if (textBox is not null) textBox.LostMouseCapture -= TextBox_LostMouseCapture;
    }

    private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        // once we've left the TextBox, return the select all behavior
        if (sender is TextBox textBox) textBox.LostMouseCapture += TextBox_LostMouseCapture;
    }
}