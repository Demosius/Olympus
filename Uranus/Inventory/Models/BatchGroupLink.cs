using SQLiteNetExtensions.Attributes;

namespace Uranus.Inventory.Models;

public class BatchGroupLink
{
    [ForeignKey(typeof(Batch))] public string BatchID { get; set; }
    [ForeignKey(typeof(BatchGroup))] public string GroupName { get; set; }

    public BatchGroupLink(string batchID, string groupName)
    {
        BatchID = batchID;
        GroupName = groupName;
    }

    public BatchGroupLink(Batch batch, BatchGroup group) : this(batch.ID, group.Name)
    {
        batch.Groups.Add(group);
        group.Batches.Add(batch);
    }
}