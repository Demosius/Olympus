namespace Morpheus.Views;

/// <summary>
/// Interaction logic for InputWindow.xaml
/// </summary>
public partial class InputWindow
{
    public string InputText => VM.Input;

    public InputWindow()
    {
        InitializeComponent();
    }

    public InputWindow(string prompt)
    {
        InitializeComponent();
        VM.Prompt = prompt;
    }

    public InputWindow(string prompt, string title)
    {
        InitializeComponent();
        VM.Prompt = prompt;
        VM.Title = title;
    }
}