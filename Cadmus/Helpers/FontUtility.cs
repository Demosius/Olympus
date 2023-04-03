using System;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Cadmus.Helpers;

internal class FontUtility
{
    public static void AddFontFromResource(PrivateFontCollection privateFontCollection, string fontResourceName)
    {
        var fontBytes = GetFontResourceBytes(typeof(App).Assembly, fontResourceName);
        var fontData = Marshal.AllocCoTaskMem(fontBytes.Length);
        Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length);
        privateFontCollection.AddMemoryFont(fontData, fontBytes.Length);
    }

    private static byte[] GetFontResourceBytes(Assembly assembly, string fontResourceName)
    {
        var resourceStream = assembly.GetManifestResourceStream(fontResourceName);
        if (resourceStream == null)
            throw new Exception($"Unable to find font '{fontResourceName}' in embedded resources.");
        var fontBytes = new byte[resourceStream.Length];
        _ = resourceStream.Read(fontBytes, 0, (int)resourceStream.Length);
        resourceStream.Close();
        return fontBytes;
    }
}