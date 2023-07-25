using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Morpheus;
using Morpheus.ViewModels.Controls;
using Quest.ViewModels;
using Styx;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;
// ReSharper disable StringLiteralTypo

namespace Quest.Views;

/// <summary>
/// Interaction logic for QuestPage.xaml
/// </summary>
public partial class QuestPage : IProject
{
    public QuestVM VM { get; set; }

    public QuestPage(Helios helios, Charon charon, ProgressBarVM progressBar)
    {
        InitializeComponent();
        VM = new QuestVM(helios, charon, progressBar);
        DataContext = VM;
    }

    public EProject Project => EProject.Quest;

    public static bool RequiresUser => true;

    public async Task RefreshDataAsync() => await VM.RefreshDataAsync();
}