using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Morpheus;

public class FileDoesNotExistException : Exception
{
    public FileDoesNotExistException() { }
    public FileDoesNotExistException(string message) : base(message) { }
}

public class WrongFileExtensionException : Exception
{
    public WrongFileExtensionException() { }
    public WrongFileExtensionException(string message) : base(message) { }
}

public static class BarcodeUtility
{
    public static string Encode128(string sourceString)
    {
        long checkSum = 0;
        int dummy;
        string? code128Barcode = null;

        if (sourceString.Length <= 0) return code128Barcode ?? "";

        // Check for valid characters
        int counter;
        for (counter = 0; counter < sourceString.Length; counter++)
        {
            var v = sourceString[counter];
            if (!(v >= 32 && v <= 126 || v == 203)) return "";

        }

        code128Barcode = "";
        var useTableB = true;
        counter = 0;

        while (counter < sourceString.Length)
        {
            int mini;
            if (useTableB)
            {
                // Check if we can switch to Table C
                mini = counter == 0 || counter + 4 == sourceString.Length ? 4 : 6;
                TestNum(ref mini, ref counter, ref sourceString);
                if (mini < 0) // Use Table C
                {
                    if (counter == 0)
                        code128Barcode = ((char)205).ToString();
                    else // Switch to table C
                        code128Barcode += (char)199;
                    useTableB = false;
                }
                else if (counter == 0)
                    code128Barcode = ((char)204).ToString(); // Starting with table B
            }

            if (!useTableB)
            {
                // We are using Table C, try to process 2 digits
                mini = 2;
                TestNum(ref mini, ref counter, ref sourceString);
                if (mini < 0) // OK for 2 digits, process it
                {
                    if (!int.TryParse(sourceString.AsSpan(counter, counter < sourceString.Length - 1 ? 2 : 1), out dummy)) dummy = 0;
                    /*if (!int.TryParse(sourceString.AsSpan(counter, counter < sourceString.Length - 1 ? 2 : 1), out dummy)) dummy = 0;*/
                    dummy = dummy < 95 ? dummy + 32 : dummy + 100;
                    code128Barcode += (char)dummy;
                    counter += 2;
                }
                else // We haven't got 2 digits, switch to Table B
                {
                    code128Barcode += (char)200;
                    useTableB = true;
                }
            }

            if (!useTableB) continue;

            // Process 1 digit with table B
            code128Barcode += sourceString[counter];
            ++counter;
        }

        // Calculation of the checksum
        for (counter = 0; counter < code128Barcode.Length; counter++)
        {
            dummy = code128Barcode[counter];
            dummy = dummy < 127 ? dummy - 32 : dummy - 100;
            if (counter == 0) checkSum = dummy;
            checkSum = (checkSum + (counter) * dummy) % 103;
        }

        // Calculation of the checksum ASCII code
        checkSum = checkSum < 95 ? checkSum + 32 : checkSum + 100;

        // Add the checksum and the STOP
        code128Barcode = code128Barcode + (char)checkSum + (char)206;

        return code128Barcode;
    }

    private static void TestNum(ref int mini, ref int counter, ref string sourceString)
    {
        // if the mini characters from Counter are numeric, then mini=0
        mini--;
        if (counter + mini >= sourceString.Length) return;

        while (mini >= 0)
        {
            if (sourceString[counter + mini] < 48 || sourceString[counter + mini] > 57)
                break;
            mini--;
        }
    }
}

public static class General
{
    public static void ShowUnexpectedException(Exception ex)
    {
        _ = MessageBox.Show($"Unexpected Exception:\n\n{ex}\n\nNotify Olympus development when possible.");
    }

    // Gets raw string data from the clipboard.
    public static string ClipboardToString()
    {
        var rawData = "";
        Thread thread = new(delegate ()
        {
            rawData = Clipboard.GetText(TextDataFormat.Text);
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        return rawData;
    }
}

/// <summary>
///  General utility functions that are likely to be shared by multiple classes.
/// </summary>
public static class Utility
{
    /// <summary>
    ///  Takes a data table and a dictionary of required columns and validates the data.
    /// </summary>
    /// <param name="data">A data table.</param>
    /// <param name="columns">
    /// A dictionary of string - string value pairs.
    /// The key is the necessary column, and the value is a backup column name.
    /// </param>
    /// <returns>
    /// False if the data does not contain the correct columns, 
    /// and cannot be renamed appropriately. (Adjusts the data as necessary.
    /// </returns>
    public static List<string> ValidateTableData(DataTable data, Dictionary<string, string> columns)
    {
        List<string> missing = new();

        foreach (var (key, value) in columns.Where(column => !data.Columns.Contains(column.Key)))
        {
            if (data.Columns.Contains(value))
            {
                data.Columns[value]!.ColumnName = key;
            }
            else
            {
                missing.Add(value);
            }
        }
        return missing;
    }

    /// <summary>
    ///  Checks if the given list of columns are contained in the data-table.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    public static List<string> ValidateTableData(DataTable data, List<string> columns)
    {
        return columns.Where(column => !data.Columns.Contains(column)).ToList();
    }

    /// <summary>
    /// Find highest common factor among a set of integers.
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
    public static int HCF(IEnumerable<int> numbers)
    {
        return numbers.Aggregate(HCF);
    }

    /// <summary>
    /// Find Highest Common Factor of two integers.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static int HCF(int a, int b)
    {
        while (true)
        {
            if (b == 0) return a;
            var a1 = a;
            a = b;
            b = a1 % b;
        }
    }
}