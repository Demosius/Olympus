using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Morpheus.ViewModels.Controls;
using Serilog;
using Uranus;
using Uranus.Staff.Models;

namespace Deimos;

public static class PickDataUtility
{
    public static async Task<List<PickEvent>> LoadPickEventsFromFileAsync(string filePath)
    {
        List<PickEvent> events;
        // Get data from file.
        try
        {
            events = await DataConversion.FileToPickEventsAsync(filePath);
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(
                $"File: {filePath}\n\nDid not contain valid Pick Event data.\n\nMissingColumns: {ex.MissingColumns}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return new List<PickEvent>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when loading pick event data from file.");
            MessageBox.Show($"Unexpected exception occurred when loading pick event data from file:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return new List<PickEvent>();
        }

        return events;
    }

    /// <summary>
    /// Loads pick event data from given files.
    ///
    /// WARNING: Pick Event data tends to be large with 10s & 100s of thousands of lines of data.
    /// It is usually best to handle one file of data at a time so prevent an unmanageable amount of memory use.
    /// </summary>
    /// <param name="files"></param>
    /// <returns>Pick Events as determined from files.</returns>
    public static async Task<List<PickEvent>> LoadPickEventsFromFilesAsync(IEnumerable<string> files)
    {
        var tasks = files.Select(LoadPickEventsFromFileAsync).ToList();

        var pickEvents = await Task.WhenAll(tasks);

        return pickEvents.SelectMany(l => l.ToList()).ToList();
    }

    public static async Task<int> UploadPickDataToDatabaseAsync(List<PickEvent> events, List<PickSession> sessions, List<PickDailyStats> stats, Helios helios)
    {
        int lines;
        
        // Upload data to database.
        try
        {
            lines = await helios.StaffUpdater.UploadPickHistoryDataAsync(events, sessions, stats);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when uploading pick event data to database.");
            MessageBox.Show($"Unexpected exception occurred when uploading pick event data to database:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }
        return lines;
    }

    /// <summary>
    /// Async method for both loading data from a given file, and then uploading it to a database.
    /// </summary>
    /// <param name="filePath">Filepath to file that should contain Pick Event Data</param>
    /// <param name="helios">Database connection object to handle db operations.</param>
    /// <returns>Tuple (event count, session count, dailyStat count, total database rows affected)</returns>
    public static async Task<(int, int, int, int)> PickHistoryFileLoadAndUpload(string filePath, Helios helios)
    {
        var events = await LoadPickEventsFromFileAsync(filePath);
        var sessions = PickEvent.GenerateStatisticsFromEvents(events, out var stats);
        return (events.Count, sessions.Count, stats.Count, await UploadPickDataToDatabaseAsync(events, sessions, stats, helios));
    }

    public static async Task<int> PickEventFileLoadAsync(Helios helios)
    {
        var fd = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Excel/CSV (*.xls*;*.csv)|*.xls*;*.csv",
            Title = "Select Pick Event Files"
        };

        if (fd.ShowDialog() != true) return 0;

        var tim = new Stopwatch();
        tim.Start();

        var files = fd.FileNames.ToList();

        Mouse.OverrideCursor = Cursors.Wait;

        var fileCount = files.Count;
        var eventCount = 0;
        var sessionCount = 0;
        var statCount = 0;
        var dbRows = 0;

        var tasks = files.Select(f => PickHistoryFileLoadAndUpload(f, helios)).ToList();

        var tup = await Task.WhenAll(tasks);

        foreach (var valueTuple in tup)
        {
            eventCount += valueTuple.Item1;
            sessionCount += valueTuple.Item2;
            statCount += valueTuple.Item3;
            dbRows += valueTuple.Item4;
        }

        tim.Stop();
        
        MessageBox.Show($"Time Taken: {tim.Elapsed:g}:\n\n" +
                        $"  • Files: \t\t\t{fileCount} \n" +
                        $"  • Events: \t\t{eventCount} \n" +
                        $"  • Sessions: \t\t{sessionCount} \n" +
                        $"  • DailyStats: \t\t{statCount} \n" +
                        $"  • DB Rows Affected: \t{dbRows} \n",
            "Upload Success", MessageBoxButton.OK,
            MessageBoxImage.Information);

        return dbRows;
    }

    public static List<MissPick> LoadMissPicksFromFile(string filePath)
    {
        List<MissPick> missPicks;
        // Get data from file.
        try
        {
            missPicks = DataConversion.FileToMissPicks(filePath);
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(
                $"File: {filePath}\n\nDid not contain valid MissPick data.\n\nMissingColumns: {string.Join("|", ex.MissingColumns)}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return new List<MissPick>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when loading miss-pick data from file.");
            MessageBox.Show($"Unexpected exception occurred when loading miss-pick data from file:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return new List<MissPick>();
        }

        return missPicks;
    }

    public static List<MissPick> LoadMissPicksFromFiles(IEnumerable<string> files, ProgressBarVM? progressBar = null)
    {
        var missPicks = new List<MissPick>();

        foreach (var file in files)
        {
            progressBar?.NewAction(file, true);
            missPicks.AddRange(LoadMissPicksFromFile(file));
        }

        return missPicks;
    }

    public static async Task<int> UploadMissPicksToDatabase(List<MissPick> missPicks, Helios helios, ProgressBarVM? progressBar = null)
    {
        progressBar?.NewTitle("Uploading to database... ");
        progressBar?.SetIndeterminate();

        int lines;
        // Upload data to database.
        try
        {
            lines = await helios.StaffCreator.UploadMissPickDataAsync(missPicks);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when uploading miss-pick data to database.");
            MessageBox.Show($"Unexpected exception occurred when uploading miss-pick data to database:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }

        progressBar?.Clear();
        return lines;
    }

    public static async Task<int> MissPickFileLoadAndUploadAsync(string filePath, Helios helios)
    {
        var missPicks = LoadMissPicksFromFile(filePath);
        return await UploadMissPicksToDatabase(missPicks, helios);
    }

    public static async Task<int> MissPickFileLoadAndUploadAsync(IEnumerable<string> files, Helios helios, ProgressBarVM? progressBar = null)
    {
        var missPicks = LoadMissPicksFromFiles(files, progressBar);
        return await UploadMissPicksToDatabase(missPicks, helios, progressBar);
    }

    public static async Task<int> MissPickFileLoad(Helios helios, ProgressBarVM? progressBar = null)
    {
        var fd = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Excel/CSV (*.xls*;*.csv)|*.xls*;*.csv",
            Title = "Select File(s) Containing MissPick Data"
        };

        if (fd.ShowDialog() != true) return 0;

        var tim = new Stopwatch();
        tim.Start();

        var files = fd.FileNames.ToList();

       // Mouse.OverrideCursor = Cursors.Wait;
        progressBar?.Activate("Loading Files: ", newMin:0, newMax:files.Count, newVal:0);
        var lines = await MissPickFileLoadAndUploadAsync(files, helios, progressBar);
        progressBar?.Deactivate();
       // Mouse.OverrideCursor = Cursors.Arrow;

        tim.Stop();

        var fileCount = files.Count;

        MessageBox.Show($"{lines} miss-pick lines successfully uploaded.\n\n" +
                        $"Elapsed: {tim.Elapsed}\n" +
                        $"File Count: {fileCount}\n",
            "Upload Success", MessageBoxButton.OK, MessageBoxImage.Information);

        return lines;
    }
}