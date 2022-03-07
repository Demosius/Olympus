using System.ComponentModel;

namespace AionClock.ViewModel;

public class ClockRejectionVM : INotifyPropertyChanged
{
    private string reason;

    public string Reason
    {
        get => reason;
        set
        {
            reason = value;
            OnPropertyChanged(nameof(Reason));
        }
    }

    public ClockRejectionVM() { }

    public ClockRejectionVM(string rejectReason)
    {
        SetReason(rejectReason);
    }

    public void SetReason(string rejectReason)
    {
        Reason = rejectReason;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}