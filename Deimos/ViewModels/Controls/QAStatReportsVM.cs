using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Morpheus.ViewModels.Controls;
using Uranus;
using Uranus.Annotations;
using Uranus.Commands;
using Uranus.Interfaces;

namespace Deimos.ViewModels.Controls;

public class QAStatReportsVM : INotifyPropertyChanged, IDBInteraction
{
    public DeimosVM Deimos { get; set; }
    public QAToolVM ParentVM { get; set; }
    public Helios Helios { get; set; }
    public ProgressBarVM ProgressBar { get; set; }

    #region INotifyPropertyChanged Members

    private QAWeeklyStatsVM weeklyStats;
    public QAWeeklyStatsVM WeeklyStats
    {
        get => weeklyStats;
        set
        {
            weeklyStats = value;
            OnPropertyChanged();
        }
    }

    private QAMonthlyStatsVM monthlyStats;
    public QAMonthlyStatsVM MonthlyStats
    {
        get => monthlyStats;
        set
        {
            monthlyStats = value;
            OnPropertyChanged();
        }
    }

    private QAYearlyStatsVM yearlyStats;
    public QAYearlyStatsVM YearlyStats
    {
        get => yearlyStats;
        set
        {
            yearlyStats = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public RefreshDataCommand RefreshDataCommand { get; set; }

    #endregion

    private QAStatReportsVM(QAToolVM parentVM)
    {
        ParentVM = parentVM;
        Deimos = ParentVM.ParentVM;
        Helios = ParentVM.Helios;
        ProgressBar = ParentVM.ProgressBar;

        weeklyStats = QAWeeklyStatsVM.CreateEmpty(Helios, ProgressBar);
        monthlyStats = QAMonthlyStatsVM.CreateEmpty(Helios, ProgressBar);
        yearlyStats = QAYearlyStatsVM.CreateEmpty(Helios, ProgressBar);

        RefreshDataCommand = new RefreshDataCommand(this);
    }

    private async Task<QAStatReportsVM> InitializeAsync()
    {
        await RefreshDataAsync();
        return this;
    }

    public static Task<QAStatReportsVM> CreateAsync(QAToolVM parentVM)
    {
        var ret = new QAStatReportsVM(parentVM);
        return ret.InitializeAsync();
    }

    public static QAStatReportsVM CreateEmpty(QAToolVM parentVM) => new(parentVM);

    public async Task RefreshDataAsync()
    {
        WeeklyStats = await QAWeeklyStatsVM.CreateAsync(Helios, ProgressBar);
        MonthlyStats = await QAMonthlyStatsVM.CreateAsync(Helios, ProgressBar);
        YearlyStats = await QAYearlyStatsVM.CreateAsync(Helios, ProgressBar);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}