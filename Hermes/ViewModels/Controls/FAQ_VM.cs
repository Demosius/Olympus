using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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