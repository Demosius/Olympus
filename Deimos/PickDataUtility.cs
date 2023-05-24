using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ExcelDataReader.Exceptions;
using Microsoft.Win32;
using Morpheus.ViewModels.Controls;
using Serilog;
using Uranus;
using Uranus.Staff.Models;
using InvalidDataException = Uranus.InvalidDataException;

namespace Deimos;

public static class PickDataUtility
{
    public static List<string> BadFileReadsMP = new();
    public static List<string> LockedFilesMP = new();
    public static List<string> NoMPData = new();

    public static List<string> BadFileReadsPE = new();
    public static List<string> LockedFilesPE = new();
    public static List<string> NoPEData = new();

    private static string BadMispickReads() => BadFileReadsMP.Count > 0 ? $"\n\nBad File Reads:\n\t{string.Join("\n\t", BadFileReadsMP)}" : "";
    private static string LockedMispickFiles() => LockedFilesMP.Count > 0 ? $"\n\nLocked Files:\n\t{string.Join("\n\t", LockedFilesMP)}" : "";
    private static string NoMissPickData() => NoMPData.Count > 0 ? $"\n\nNo Data Found:\n\t{string.Join("\n\t", NoMPData)}" : "";
    private static string BadPickEventReads() => BadFileReadsPE.Count > 0 ? $"\n\nBad File Reads:\n\t{string.Join("\n\t", BadFileReadsPE)}" : "";
    private static string LockedPickEventFiles() => LockedFilesPE.Count > 0 ? $"\n\nLocked Files:\n\t{string.Join("\n\t", LockedFilesPE)}" : "";
    private static string NoPickEventData() => NoPEData.Count > 0 ? $"\n\nNo Data Found:\n\t{string.Join("\n\t", NoPEData)}" : "";

    public static void ResetMPFileData()
    {
        BadFileReadsMP = new List<string>();
        LockedFilesMP = new List<string>();
        NoMPData = new List<string>();
    }

    public static void ResetPEFileData()
    {
        BadFileReadsPE = new List<string>();
        LockedFilesPE = new List<string>();
        NoPEData = new List<string>();
    }

    public static async Task<List<PickEvent>> LoadPickEventsFromFileAsync(string filePath)
    {
        // Get data from file.
        try
        {
            var events = await DataConversion.FileToPickEventsAsync(filePath).ConfigureAwait(false);
            return events;
        }
        catch (InvalidPasswordException)
        {
            LockedFilesPE.Add(filePath);
            return new List<PickEvent>();
        }
        catch (InvalidDataException)
        {
            NoPEData.Add(filePath);
            return new List<PickEvent>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when loading pick event data from file.");
            BadFileReadsPE.Add(filePath);
            return new List<PickEvent>();
        }

    }

    /// <summary>
    /// Loads pick event data from given files.
    /// 
    /// WARNING: Pick Event data tends to be large with 10s & 100s of thousands of lines of data.
    /// It is usually best to handle one file of data at a time so prevent an unmanageable amount of memory use.
    /// </summary>
    /// <param name="files">File path strings representing files containing pick event data.</param>
    /// <param name="progress">To track progress.</param>
    /// <param name="lineCap">Used to prevent excessive memory usage.</param>
    /// <param name="startCount">Number of files already checked before this iteration.</param>
    /// <returns>Tuple: (Pick Events as determined from files., Files yet to check for data.)</returns>
    public static async Task<(List<PickEvent> events, List<string> files)> LoadPickEventsFromFilesAsync(IEnumerable<string> files, IProgress<ProgressTaskVM>? progress, int lineCap = 1000000, int startCount = 0)
    {
        var fileList = files.ToList();

        var events = new List<PickEvent>();

        var count = startCount;
        var total = fileList.Count + startCount;

        while (fileList.Any() && events.Count < lineCap)
        {
            var file = fileList.First();
            var fileName = Path.GetFileName(file);
            progress?.Report(new ProgressTaskVM("Pulling pick events from files...", fileName, 0, total, count));
            fileList.Remove(file);
            count++;

            events.AddRange(await LoadPickEventsFromFileAsync(file).ConfigureAwait(false));
        }

        return (events, fileList);
    }

    public static async Task<int> UploadPickDataToDatabaseAsync(List<PickEvent> events, List<PickSession> sessions, List<PickDailyStats> stats, Helios helios)
    {
        try
        {
            var lines = await helios.StaffUpdater.PickStatsAsync(events, sessions, stats).ConfigureAwait(false);
            return lines;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when uploading pick event data to database.");
            MessageBox.Show($"Unexpected exception occurred when uploading pick event data to database:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            throw;
        }
    }

    public static async Task<(int eventCount, int sessionCount, int statCount, int dbRowCount)> PickEventFilesLoadAndUploadAsync(List<string> files, Helios helios,
        IProgress<ProgressTaskVM>? progress)
    {
        var total = files.Count;
        var count = 0;
        var lines = 0;
        var eventCount = 0;
        var sessionCount = 0;
        var statCount = 0;

        while (files.Any())
        {
            progress?.Report(new ProgressTaskVM("Pulling pick events from files...", "", 0, total, count));
            (var events, files) = await LoadPickEventsFromFilesAsync(files, progress, startCount: count);

            count = total - files.Count;

            progress?.Report(new ProgressTaskVM("Configuring history from events...", "", 0, 0, 0));

            (events, var sessions, var stats) = await PickEvent.GenerateStatisticsFromEventsAsync(events);
            eventCount += events.Count;
            sessionCount += sessions.Count;
            statCount += stats.Count;

            progress?.Report(new ProgressTaskVM("Uploading Pick History to database...", $"e-{events.Count} | s-{sessions.Count} | d-{stats.Count}", 0, 0, 0));

            lines += await UploadPickDataToDatabaseAsync(events, sessions, stats, helios);
        }

        return (eventCount, sessionCount, statCount, lines);
    }

    /// <summary>
    /// Async method for both loading data from a given file, and then uploading it to a database.
    /// </summary>
    /// <param name="filePath">Filepath to file that should contain Pick Event Data</param>
    /// <param name="helios">Database connection object to handle db operations.</param>
    /// <returns>Tuple (event count, session count, dailyStat count, total database rows affected)</returns>
    public static async Task<(int, int, int, int)> PickHistoryFileLoadAndUploadAsync(string filePath, Helios helios)
    {
        var events = await LoadPickEventsFromFileAsync(filePath).ConfigureAwait(false);
        var sessions = PickEvent.GenerateStatisticsFromEvents(ref events, out var stats);
        return (events.Count, sessions.Count, stats.Count, await UploadPickDataToDatabaseAsync(events, sessions, stats, helios).ConfigureAwait(false));
    }

    /// <summary>
    /// Async method for loading pick events from a file, and then uploading the basic data to a database.
    /// </summary>
    /// <param name="filePath">Filepath to file containing Pick Event Data</param>
    /// <param name="helios">Database connection object to handle db operations.</param>
    /// <param name="progress">Object for tracking progress.</param>
    /// <returns>Tuple (db row count, list of dates of pick data)</returns>
    public static async Task<(int, List<DateTime>)> PickEventFileLoadAndUploadAsync(string filePath, Helios helios, IProgress<ProgressTaskVM>? progress)
    {
        var file = Path.GetFileName(filePath);
        progress?.Report(new ProgressTaskVM("Pick Events: Loading from file...", file, 0, 0, 0));
        var events = await LoadPickEventsFromFileAsync(filePath).ConfigureAwait(false);
        PickEvent.HandleDuplicateValues(ref events);
        progress?.Report(new ProgressTaskVM("Pick Events: Uploading to DB...", file, 0, 0, 0));
        var lines = await helios.StaffUpdater.PickEventsAsync(events).ConfigureAwait(false);
        return (lines, events.Select(e => e.Date).Distinct().ToList());
    }

    public static async Task<int> PickEventFileLoadAsync(Helios helios, IProgress<ProgressTaskVM>? progress)
    {
        var fd = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Excel/CSV (*.xls*;*.csv)|*.xls*;*.csv",
            Title = "Select Pick Event Files"
        };

        if (fd.ShowDialog() != true) return 0;

        var files = fd.FileNames.ToList();

        if (files.Count > 1)
        {
            var result = MessageBox.Show(
                "This process may take a while and will continue in the background.\n\nAdditional database operations may be negatively affected until process is complete.",
                "Warning: Heavy Process", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            if (result != MessageBoxResult.OK) return 0;
        }

        var tim = new Stopwatch();
        tim.Start();

        var fileCount = files.Count;

        ResetPEFileData();

        progress?.Report(new ProgressTaskVM($"Uploading {fileCount} pick event files...", "", 0, 0, 0));
        var (eventCount, sessionCount, statCount, dbRows) = await PickEventFilesLoadAndUploadAsync(files, helios, progress);
        progress?.Report(new ProgressTaskVM("Finalising pick event uploads...", "", 0, 0, 0));

        tim.Stop();

        CheckPEFiles(files.Count);

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

    private static void CheckPEFiles(int fileCount)
    {
        var badCount = NoPEData.Count + BadFileReadsPE.Count + LockedFilesPE.Count;

        if (badCount <= 0) return;

        if (fileCount <= badCount)
            MessageBox.Show(
                $"Failed to gather data from selected file(s):{BadPickEventReads()}{NoPickEventData()}{LockedPickEventFiles()}",
                "Upload failed.", MessageBoxButton.OK, MessageBoxImage.Error);
        else
            MessageBox.Show(
                $"{badCount} file(s) failed to yield appropriate data:{BadPickEventReads()}{NoPickEventData()}{LockedPickEventFiles()}" +
                "\n\nCurrently checking new data against existing and establishing pick history statistics...",
                "File(s) Failed", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public static List<Mispick> LoadMispicksFromFile(string filePath)
    {
        List<Mispick> mispicks;
        // Get data from file.
        try
        {
            mispicks = DataConversion.FileToMispicks(filePath);
        }
        catch (InvalidDataException ex)
        {
            MessageBox.Show(
                $"File: {filePath}\n\nDid not contain valid Mispick data.\n\nMissingColumns: {string.Join("|", ex.MissingColumns)}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return new List<Mispick>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when loading mispick data from file.");
            MessageBox.Show($"Unexpected exception occurred when loading mispick data from file:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return new List<Mispick>();
        }

        return mispicks;
    }

    public static async Task<List<Mispick>> LoadMispicksFromFileAsync(string filePath)
    {
        try
        {
            // Get data from file.
            var mispicks = await DataConversion.FileToMispicksAsync(filePath).ConfigureAwait(false);
            return mispicks;
        }
        catch (InvalidPasswordException)
        {
            LockedFilesMP.Add(filePath);
            return new List<Mispick>();
        }
        catch (InvalidDataException)
        {
            NoMPData.Add(filePath);
            return new List<Mispick>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when loading mispick data from file.");
            BadFileReadsMP.Add(filePath);
            return new List<Mispick>();
        }
    }

    public static async Task<List<Mispick>> LoadMispicksFromFilesAsync(List<string> files, IProgress<ProgressTaskVM>? progress = null)
    {
        ResetMPFileData();

        var count = 0;
        var total = files.Count;

        var mispicks = new List<Mispick>();

        foreach (var file in files)
        {
            progress?.Report(new ProgressTaskVM("Loading mispick files...", Path.GetFileName(file), min: 0, max: total, val: count));

            mispicks.AddRange(await LoadMispicksFromFileAsync(file));
            count++;
        }

        return mispicks;
    }

    public static async Task<int> UploadMispicksToDatabaseAsync(List<Mispick> mispicks, Helios helios, IProgress<ProgressTaskVM>? progress = null)
    {
        try
        {
            progress?.Report(new ProgressTaskVM($"Uploading {mispicks.Count} mispicks to database...", string.Empty, min: 0, max: 0, val: 0));
            // Upload data to database.
            var lines = await helios.StaffCreator.UploadMispickDataAsync(mispicks).ConfigureAwait(false);
            return lines;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unknown error when uploading mispick data to database.");
            MessageBox.Show($"Unexpected exception occurred when uploading mispick data to database:\n\n{ex}", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return 0;
        }
    }

    public static async Task<int> MispickFileLoadAndUploadAsync(string filePath, Helios helios)
    {
        var mispicks = await LoadMispicksFromFileAsync(filePath).ConfigureAwait(false);
        return await UploadMispicksToDatabaseAsync(mispicks, helios).ConfigureAwait(false);
    }

    public static async Task<int> MispickFileLoadAndUploadAsync(List<string> files, Helios helios, IProgress<ProgressTaskVM>? progress = null)
    {
        var mispicks = await LoadMispicksFromFilesAsync(files, progress).ConfigureAwait(false);
        Mispick.HandleDuplicateValues(ref mispicks);
        var lines = await UploadMispicksToDatabaseAsync(mispicks, helios, progress).ConfigureAwait(false);

        if (!NoMPData.Any() && !BadFileReadsMP.Any() && !LockedFilesMP.Any())
            return lines;

        var count = NoMPData.Count + BadFileReadsMP.Count + LockedFilesMP.Count;

        if (files.Count <= count)
        {
            MessageBox.Show(
                $"Failed to gather data from selected file(s):\n{BadMispickReads()}{NoMissPickData()}{LockedMispickFiles()}",
                "Upload failed.", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show(
                $"{count} file(s) failed to yield appropriate data:\n{BadMispickReads()}{NoMissPickData()}{LockedMispickFiles()}" +
                $"\n\nNow uploading {mispicks.Count} lines of data to the database...",
                "File(s) Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return lines;
    }

    public static async Task<int> MispickFileLoadAsync(Helios helios, IProgress<ProgressTaskVM>? progress = null)
    {
        var fd = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Excel/CSV (*.xls*;*.csv)|*.xls*;*.csv",
            Title = "Select File(s) Containing Mispick Data"
        };

        if (fd.ShowDialog() != true) return 0;

        var tim = new Stopwatch();
        tim.Start();

        var files = fd.FileNames.ToList();

        progress?.Report(new ProgressTaskVM("Mispick file load and upload...", "", 0, 0, 0));
        var lines = await MispickFileLoadAndUploadAsync(files, helios, progress).ConfigureAwait(false);

        tim.Stop();

        var fileCount = files.Count;

        MessageBox.Show($"{lines} mispick lines successfully uploaded.\n\n" +
                        $"Elapsed: {tim.Elapsed}\n" +
                        $"File Count: {fileCount}\n",
            "Upload Success", MessageBoxButton.OK, MessageBoxImage.Information);

        return lines;
    }
}