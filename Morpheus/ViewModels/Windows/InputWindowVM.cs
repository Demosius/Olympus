using Morpheus.ViewModels.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Morpheus.ViewModels.Windows;

public class InputWindowVM : INotifyPropertyChanged
{
    private string title;
    public string Title
    {
        get => title;
        set
        {
            title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    private string prompt;
    public string Prompt
    {
        get => prompt;
        set
        {
            prompt = value;
            OnPropertyChanged(nameof(Prompt));
        }
    }

    private string input;
    public string Input
    {
        get => input;
        set
        {
            input = value;
            OnPropertyChanged(nameof(Input));
        }
    }

    public ConfirmInputCommand ConfirmInputCommand { get; set; }

    public InputWindowVM()
    {
        title = "Input Window";
        prompt = "Enter Text";
        input = string.Empty;

        ConfirmInputCommand = new ConfirmInputCommand();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}