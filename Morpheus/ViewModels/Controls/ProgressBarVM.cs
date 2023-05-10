using System.ComponentModel;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Morpheus.ViewModels.Controls;

public class ProgressBarVM : INotifyPropertyChanged
{
    #region INotifyPropertyChanged Members

    private string title;
    public string Title
    {
        get => title;
        set
        {
            title = value;
            OnPropertyChanged();
        }
    }

    private string action;
    public string Action
    {
        get => action;
        set
        {
            action = value;
            OnPropertyChanged();
        }
    }

    private int min;
    public int Min
    {
        get => min;
        set
        {
            min = value;
            OnPropertyChanged();
            SetPercent();
        }
    }

    private int max;
    public int Max
    {
        get => max;
        set
        {
            max = value;
            OnPropertyChanged();
            SetPercent();
        }
    }

    private bool isIndeterminate;
    public bool IsIndeterminate
    {
        get => isIndeterminate;
        set
        {
            isIndeterminate = value;
            OnPropertyChanged();
        }
    }

    private int val;
    public int Val
    {
        get => val;
        set
        {
            val = value;
            OnPropertyChanged();
            SetPercent();
        }
    }

    private bool isActive;
    public bool IsActive
    {
        get => isActive;
        set
        {
            isActive = value;
            OnPropertyChanged();
        }
    }

    private double pct;
    public double Pct
    {
        get => pct;
        set
        {
            pct = value;
            OnPropertyChanged();
        }
    }

    private bool showPct;
    public bool ShowPct
    {
        get => showPct && !isIndeterminate;
        set
        {
            showPct = value;
            OnPropertyChanged();
        }
    }

    #endregion

    public ProgressBarVM()
    {
        title = string.Empty;
        action = string.Empty;
    }

    public void Activate(string newTitle = "", string newAction = "", bool determinative = false, int newMin = 0, int newMax = 0,
        int newVal = 0, bool showPercent = false)
    {
        IsActive = true;
        Title = newTitle;
        Action = newAction;
        IsIndeterminate = determinative;
        Min = newMin;
        Max = newMax;
        Val = newVal;
        ShowPct = showPercent;
    }

    public void NewTitle(string newTitle, string newAction = "", bool inc = false)
    {
        Title = newTitle;
        Action = newAction;
        if (inc && Max > 0) Inc();
    }

    public void NewAction(string newAction, bool inc = false)
    {
        Action = newAction;
        if (inc && Max > 0) Inc();
    }

    public void Deactivate() => Clear();

    public void Inc(int i = 1)
    {
        Val += i;
    }

    private void SetPercent()
    {
        var tMax = Max - Min;
        var tVal = Val - Min;
        Pct = tVal / (tMax == 0 ? 1 : tMax) * 100;
    }

    public void SetIndeterminate(bool isNowIndeterminate = true)
    {
        IsIndeterminate = isNowIndeterminate;
    }

    public void Clear()
    {
        Min = 0;
        Max = 0;
        Val = 0;
        Title = string.Empty;
        Action = string.Empty;
        IsIndeterminate = false;
        IsActive = false;
        ShowPct = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}