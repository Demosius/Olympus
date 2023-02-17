using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uranus;

/// <summary>
///  Exception for when a table of data is missing appropriate columns.
/// </summary>
public class InvalidDataException : Exception
{
    public List<string> MissingColumns { get; }

    public InvalidDataException(string message, List<string> missingColumns) : base($"Missing Columns:\n\n{string.Join("|", missingColumns)}{(message != "" ? $"\n\n{message}" : message)}")
    {
        MissingColumns = missingColumns;
    }

    public InvalidDataException(List<string> missingColumns)
    {
        MissingColumns = missingColumns;
    }
}

/// <summary>
///  Exception for when a database connection has failed.
/// </summary>
public class FailedConnectionException : Exception
{
    public FailedConnectionException(string message) : base(message) { }
}

/// <summary>
///  General utility functions that are likely to be shared by multiple classes.
/// </summary>
public static class Utility
{
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

    /// <summary>
    /// Find and return the longest common substring of two given strings.
    /// </summary>
    /// <param name="str1"></param>
    /// <param name="str2"></param>
    /// <returns></returns>
    public static string LongestCommonSubstring(string str1, string str2)
    {
        if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2))
            return string.Empty;

        var num = new int[str1.Length, str2.Length];
        var maxLen = 0;
        var lastSubsBegin = 0;
        var subStrBuilder = new StringBuilder();

        for (var i = 0; i < str1.Length; i++)
        {
            for (var j = 0; j < str2.Length; j++)
            {
                if (str1[i] != str2[j])
                {
                    num[i, j] = 0;
                }
                else
                {
                    if ((i == 0) || (j == 0))
                        num[i, j] = 1;
                    else
                        num[i, j] = 1 + num[i - 1, j - 1];

                    if (num[i, j] <= maxLen) continue;

                    maxLen = num[i, j];

                    var thisSubsBegin = i - num[i, j] + 1;

                    if (lastSubsBegin == thisSubsBegin)
                    {
                        subStrBuilder.Append(str1[i]);
                    }
                    else
                    {
                        lastSubsBegin = thisSubsBegin;
                        subStrBuilder.Length = 0;
                        subStrBuilder.Append(str1.AsSpan(lastSubsBegin, (i + 1) - lastSubsBegin));
                    }
                }
            }
        }
        
        return subStrBuilder.ToString();
    }

    /// <summary>
    /// Get longest common substring among group of strings.
    /// </summary>
    /// <param name="strings"></param>
    /// <returns></returns>
    public static string LongestCommonSubstring(IEnumerable<string> strings)
    {
        return strings.Aggregate(LongestCommonSubstring);
    }
}