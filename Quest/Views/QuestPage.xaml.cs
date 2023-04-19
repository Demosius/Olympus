using System;
using System.Linq;
using System.Windows;
using Morpheus;
using Quest.ViewModels;
using Styx;
using Uranus;
using Uranus.Interfaces;
using Uranus.Staff;

namespace Quest.Views;

/// <summary>
/// Interaction logic for QuestPage.xaml
/// </summary>
public partial class QuestPage : IProject
{
    public QuestVM VM { get; set; }

    public QuestPage(Helios helios, Charon charon)
    {
        InitializeComponent();
        VM = new QuestVM(helios, charon);
        DataContext = VM;
    }

    public EProject Project => EProject.Quest;

    public static bool RequiresUser => true;

    public void RefreshData() => VM.RefreshData();
    

    private void UploadButton_OnClick(object sender, RoutedEventArgs e)
    {
        var raw = General.ClipboardToString();

        var lines = VM.Helios.StaffUpdater.UploadPickEvents(raw);

        MessageBox.Show($"{lines} lines affected by the upload.");
    }

    private void DownloadButton_OnClick(object sender, RoutedEventArgs e)
    {
        var helios = new Helios("\\\\ausefpdfs01ns\\Shares\\Public\\DC_Data\\Olympus\\QA\\Sol");

        var stats = helios.StaffReader.PickStats(new DateTime(2020, 1, 1), DateTime.Today, true).ToList();
        var sessions = stats.SelectMany(s => s.PickSessions).ToList();
        var events = stats.SelectMany(s => s.PickEvents).ToList();

        MessageBox.Show("Download Complete:\n\n" +
                        $"{events.Count} Pick Events\n" +
                        $"{sessions.Count} Pick Sessions\n" +
                        $"{stats.Count} Pick Stats");
    }
}