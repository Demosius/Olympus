using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;
using Uranus.Users.Models;

namespace Hermes.ViewModels.Controls;

public class FAQ_VM : INotifyPropertyChanged
{
    private readonly FAQ faq;

    #region FAQ Access

    public string Question
    {
        get => faq.Question;
        set
        {
            faq.Question = value;
            OnPropertyChanged();
        }
    }

    public string Answer
    {
        get => faq.Answer;
        set
        {
            faq.Answer = value;
            OnPropertyChanged(); 
        }
    }

    #endregion
    public FAQ_VM(FAQ faq)
    {
        this.faq = faq;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}