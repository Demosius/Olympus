using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uranus.Inventory.Models;

namespace Uranus.Inventory;

public class InventoryDeleter
{
    public InventoryChariot Chariot { get; set; }

    public InventoryDeleter(ref InventoryChariot chariot)
    {
        Chariot = chariot;
    }

    // Deletes stock where zones are in the given list.
    public void StockZoneDeletes(List<string> zoneIDs) => Chariot.StockZoneDeletes(zoneIDs);

    public int Site(Site site) => Chariot.Delete(site);

    public async Task<int> BatchTOGroupAsync(BatchTOGroup group) => await Task.Run(() => Chariot.Delete(group)).ConfigureAwait(false);

    public async Task<int> BatchTOGroupsAsync(List<BatchTOGroup> groups)
    {
        var lines = 0;

        void Action()
        {
            lines += groups.Sum(group => Chariot.Delete(group));
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return lines;
    }

    /// <summary>
    /// With the original file information, delete all relevant lines from database
    /// along with all newly empty groups.
    /// </summary>
    /// <param name="ogDir"></param>
    /// <param name="ogFileName"></param>
    /// <returns>List of filePaths of files created from the original.</returns>
    public async Task<List<string>> BatchFileRecovery(string ogDir, string ogFileName)
    {
        var files = new List<string>();

        void Action()
        {
            // Pull relevant lines.
            var lines = Chariot.PullObjectList<BatchTOLine>(l =>
                l.OriginalFileDirectory == ogDir && l.OriginalFileName == ogFileName);

            // Get group IDs
            var groupIDs = lines.Select(l => l.GroupID).Distinct().ToList();
            
            // Delete lines while getting any potential new file data.
            foreach (var line in lines)
            {
                var newFile = Path.Join(line.FinalFileDirectory, line.FinalFileName);
                if (File.Exists(newFile))
                    files.Add(newFile);
                Chariot.Delete(line);
            }

            // Delete group if no lines remain with group id.
            foreach (var groupID in groupIDs
                         .Where(groupID => Chariot.ExecuteScalar<int>("SELECT COUNT(*) FROM BatchTOLine WHERE GroupID = ?;", groupID) == 0))
            {
                Chariot.DeleteByKey<BatchTOGroup>(groupID);
            }
        }

        await Task.Run(() => Chariot.RunInTransaction(Action)).ConfigureAwait(false);

        return files.Distinct().ToList();
    }
}