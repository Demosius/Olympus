using System;
using System.IO;
using System.Text.Json;

namespace Aion.ViewModels.Utility;

public class Options
{
    private static readonly string jsonFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "options.json");

    public string DBLocation { get; set; }

    public Options()
    {
        var data = File.ReadAllText(jsonFile);
        var o = JsonSerializer.Deserialize<Opt>(data);

        DBLocation = string.Empty;

        // Set properties.
        if (o != null) DBLocation = o.DBLocation;
    }

    public static string GetDBLocation()
    {
        var data = File.ReadAllText(jsonFile);
        return JsonSerializer.Deserialize<Options>(data)?.DBLocation ?? string.Empty;
    }

    public void LoadOptions()
    {
        SetFromOther(new Options());
    }

    public void SaveOptions()
    {
        var data = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonFile, data);
    }

    public void SetFromOther(Options options)
    {
        DBLocation = options.DBLocation;
    }
}

/// <summary>
/// Intermediate option class for converting to and from JSON
/// </summary>
public class Opt
{
    public string DBLocation { get; set; }

    public Opt()
    {
        DBLocation = string.Empty;
    }
}