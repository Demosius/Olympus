using System.Windows;
using Olympus.ViewModels.Windows;

namespace Olympus.Views.Windows;

/// <summary>
/// Interaction logic for BinContentsUpdaterWindow.xaml
/// </summary>
public partial class BinContentsUpdaterWindow
{
    public BinContentsUpdaterVM VM { get; set; }

    public int UploadedLines => VM.SuccessfulUploadLines;

    public BinContentsUpdaterWindow(BinContentsUpdaterVM vm)
    {
        VM = vm;

        InitializeComponent();
        DataContext = VM;
    }

    private void ClearNoneButton_OnClick(object sender, RoutedEventArgs e)
    {
        // Deselect, confirm, close.
        VM.DeselectAll();
        VM.Confirm();
        DialogResult = true;
        Close();
    }

    private void ClearAllButton_OnClick(object sender, RoutedEventArgs e)
    {
        VM.SelectAll();
        VM.Confirm();
        DialogResult = true;
        Close();
    }
}