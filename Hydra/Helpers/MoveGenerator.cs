using Hydra.ViewModels.Controls;
using System.Collections.Generic;
using Uranus.Inventory.Models;

namespace Hydra.Helpers;

public static class MoveGenerator
{
    public static IEnumerable<MoveVM> GenerateSiteMoves(HydraDataSet dataSet, IEnumerable<string> fromSites, IEnumerable<string> toSites)
    {
        var returnList = new List<MoveVM>();

        // Set Site Objects
        var takeSites = new List<Site>();
        var placeSites = new List<Site>();

        foreach (var siteName in fromSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) takeSites.Add(site);
        foreach (var siteName in toSites)
            if (dataSet.Sites.TryGetValue(siteName, out var site)) placeSites.Add(site);

        // Generate Site Specific Zones/Bays/Bins
        foreach (var (name, site) in dataSet.Sites)
        {
            var locationCode = $"{name}SiteLocation";
            var zoneCode = $"{name}SiteZone";
            var bayCode = $"{name}SiteBay";
            var binCode = $"{name}SiteBin";

            var location = new NAVLocation(locationCode, locationCode);
            var zone = new NAVZone(zoneCode, location);
            var bay = new Bay();
            var bin = $"{name}SiteBin";

        }

        return returnList;
    }
}