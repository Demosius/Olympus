using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Uranus.Annotations;

namespace Morpheus.ViewModels.Controls;

public class ProgressBarVM : INotifyPropertyChanged
{
    public Progress<ProgressTaskVM> Progress { get; }

    public int Tasks { get; set; }

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
        }
    }

    private string pct;
    public string Pct
    {
        get => pct;
        set
        {
            pct = value;
            OnPropertyChanged();
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

    private bool showPct;
    public bool ShowPct
    {
        get => showPct;
        set
        {
            showPct = value;
            OnPropertyChanged();
        }
    }

    public bool IsActive => Tasks > 0;

    #endregion

    public ProgressBarVM()
    {
        Tasks = 0;
        Progress = new Progress<ProgressTaskVM>(ReportProgress);

        title = string.Empty;
        action = string.Empty;
        pct = string.Empty;
    }

    public IProgress<ProgressTaskVM> StartTask(string newTitle, string newAction = "", int newMin = 0, int newMax = 0, int newVal = 0)
    {
        Tasks++;

        OnPropertyChanged(nameof(IsActive));

        ((IProgress<ProgressTaskVM>)Progress).Report(new ProgressTaskVM(newTitle, newAction, newMin, newMax, newVal));

        return Progress;
    }

    public void EndTask()
    {
        Tasks--;

        if (Tasks < 0) Tasks = 0;

        OnPropertyChanged(nameof(IsActive));
    }

    private void ReportProgress(ProgressTaskVM task)
    {
        Title = task.Title ?? Title;
        Action = task.Action ?? Action;
        Min = task.Min ?? Min;
        Max = task.Max ?? Max;
        Val = task.Val ?? Val;

        IsIndeterminate = Max == 0;
        ShowPct = !IsIndeterminate;

        var pctVal = IsIndeterminate ? 0 : (double)(Val - Min) / (Max - Min);

        Pct = $"{pctVal * 100:#0.0#}%";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}