using Morpheus.ViewModels.Windows;

namespace Morpheus.Views.Windows;

/// <summary>
/// Interaction logic for InputWindow.xaml
/// </summary>
public partial class InputWindow
{
    public InputWindowVM VM { get; set; }

    public string InputText => VM.Input;

    public InputWindow()
    {
        InitializeComponent();
        VM = new InputWindowVM();
        DataContext = VM;
    }

    public InputWindow(string prompt)
    {
        InitializeComponent();
        VM = new InputWindowVM(prompt);
        DataContext = VM;
    }

    public InputWindow(string prompt, string title)
    {
        InitializeComponent();
        VM = new InputWindowVM(prompt, title);
        DataContext = VM;
    }
}