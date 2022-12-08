using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Cadmus;

public class Printing
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateFile(string lpFileName, FileAccess dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, FileMode dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    public static void PrintToZebra()
    {
        // Command to be sent to the printer
        // ReSharper disable once StringLiteralTypo
        var command = "^XA^FO10,10,^AO,30,20^FDFDTesting^FS^FO10,30^BY3^BCN,100,Y,N,N^FDTesting^FS^XZ";

        // Create a buffer with the command
        var buffer = Encoding.ASCII.GetBytes(command);

        // Use the CreateFile external func to connect to the LPT1 port
        var printer = CreateFile("LPT1:", FileAccess.ReadWrite, 0, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

        // Here I check if the printer is valid.
        if (printer.IsInvalid) return;

        // Open the fileStream to the lpt1 port and send the command
        var lpt1 = new FileStream(printer, FileAccess.ReadWrite);
        lpt1.Write(buffer, 0, buffer.Length);

        // Close the FileStream connection
        lpt1.Close();

    }
}