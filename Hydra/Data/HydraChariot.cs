using System;
using System.Collections.Generic;
using System.IO;
using Uranus;
using Uranus.Inventory.Models;

namespace Hydra.Data;

public sealed class HydraChariot : MasterChariot
{
    public override string DatabaseName { get; }
    public override Type[] Tables { get; } =
    {
        typeof(Bay), typeof(BayZone), typeof(BinExtension), typeof(Move),
        typeof(NAVBin), typeof(NAVItem), typeof(NAVLocation), typeof(NAVStock),
        typeof(NAVUoM), typeof(NAVZone), typeof(Stock), typeof(Store),
        typeof(SubStock), typeof(ZoneExtension), typeof(ItemExtension), typeof(Site),
        typeof(SiteItemLevel)
    };

    public HydraChariot(string fullDatabasePath)
    {
        BaseDataDirectory = Path.GetDirectoryName(fullDatabasePath) ?? "";
        DatabaseName = Path.GetFileName(fullDatabasePath);
        InitializeDatabaseConnection();

    }

    public HydraChariot(string databaseDirectory, string databaseName)
    {
        BaseDataDirectory = databaseDirectory;
        DatabaseName = databaseName;
        InitializeDatabaseConnection();
    }

    public int SendData(HydraDataSet dataSet, IEnumerable<Move> moves)
    {
        var lines = 0;

        Database?.RunInTransaction(() =>
        {
            lines += UpdateTable(moves);
            lines += UpdateTable(dataSet.Stock);
            lines += UpdateTable(dataSet.Zones.Values);
            lines += UpdateTable(dataSet.Items.Values);
            lines += UpdateTable(dataSet.Bins.Values);
            lines += UpdateTable(dataSet.Locations.Values);
            lines += UpdateTable(dataSet.SiteItemLevels.Values);
            lines += UpdateTable(dataSet.Sites.Values);
            lines += UpdateTable(dataSet.NAVStock);
            lines += UpdateTable(dataSet.UoMs);
        });

        return lines;
    }

}