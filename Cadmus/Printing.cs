using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
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

    public static string SendImageToPrinter(int top, int left, Bitmap bitmap)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms, Encoding.ASCII);
        //we set p3 parameter, remember it is Width of Graphic in bytes,
        //so we divide the width of image and round up of it
        var p3 = (int)Math.Ceiling((double)bitmap.Width / 8);
        bw.Write(Encoding.ASCII.GetBytes($"GW{top},{left},{p3},{bitmap.Height},"));
        //the width of matrix is rounded up multi of 8
        var canvasWidth = p3 * 8;
        //Now we convert image into 2 dimension binary matrix by 2 for loops below,
        //in the range of image, we get colour of pixel of image,
        //calculate the luminance in order to set value of 1 or 0
        //otherwise we set value to 1
        //Because P3 is set to byte (8 bits), so we gather 8 dots of this matrix,
        //convert into a byte then write it to memory by using shift left operator <<
        //e,g 1 << 7  ---> 10000000
        //    1 << 6  ---> 01000000
        //    1 << 3  ---> 00001000
        for (var y = 0; y < bitmap.Height; ++y)     //loop from top to bottom
        {
            for (var x = 0; x < canvasWidth;)       //from left to right
            {
                byte aByte = 0;
                for (var b = 0; b < 8; ++b, ++x)     //get 8 bits together and write to memory
                {
                    var dot = 1;                     //set 1 for white,0 for black
                    //pixel still in width of bitmap,
                    //check luminance for white or black, out of bitmap set to white
                    if (x < bitmap.Width)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));
                        dot = luminance > 127 ? 1 : 0;
                    }
                    aByte |= (byte)(dot << (7 - b)); //shift left,
                    //then OR together to get 8 bits into a byte
                }
                bw.Write(aByte);
            }
        }
        bw.Write("\n");
        bw.Flush();
        //reset memory
        ms.Position = 0;
        //get encoding, I have no idea why encode page of 1252 works and fails for others
        var v = ms.ToArray();
        return Encoding.GetEncoding(1252).GetString(v);
    }

    private static Bitmap RotateImg(Image bmp, float angle)
    {
        angle %= 360;
        if (angle > 180) angle -= 360;
        var sin = (float)Math.Abs(Math.Sin(angle *
            Math.PI / 180.0)); // this function takes radians
        var cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
        var newImgWidth = sin * bmp.Height + cos * bmp.Width;
        var newImgHeight = sin * bmp.Width + cos * bmp.Height;
        var originX = 0f;
        var originY = 0f;
        switch (angle)
        {
            case > 0 and <= 90:
                originX = sin * bmp.Height;
                break;
            case > 0:
                originX = newImgWidth;
                originY = newImgHeight - sin * bmp.Width;
                break;
            case >= -90:
                originY = sin * bmp.Width;
                break;
            default:
                originX = newImgWidth - sin * bmp.Height;
                originY = newImgHeight;
                break;
        }
        var newImg =
            new Bitmap((int)newImgWidth, (int)newImgHeight);
        var g = Graphics.FromImage(newImg);
        g.Clear(Color.White);
        g.TranslateTransform(originX, originY); // offset the origin to our calculated values
        g.RotateTransform(angle); // set up rotate
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
        g.Dispose();
        return newImg;
    }

    public static string SendImageToPrinter(int top, int left, string source, float angle)
    {
        var bitmap = (Bitmap)Image.FromFile(source);
        var newBitmap = RotateImg(bitmap, angle);
        return SendImageToPrinter(top, left, newBitmap);
    }

    public static string SendImageToPrinter(int top, int left, string source)
    {
        var bitmap = (Bitmap)Image.FromFile(source);
        return SendImageToPrinter(top, left, bitmap);
    }
}