using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios.Inventory.Model
{
    public class Move
    {
        [PrimaryKey]
        public Guid ID { get; set; }
        [ForeignKey(typeof(NAVItem))]
        public int ItemNumber { get; set; }
        [ForeignKey(typeof(NAVBin))]
        public string TakeBinID { get; set; }
        [ForeignKey(typeof(NAVBin))]
        public string PlaceBinID { get; set; }
        [ForeignKey(typeof(Batch))]
        public string BatchID { get; set; }

        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVItem Item { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin TakeBin { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public NAVBin PlaceBin { get; set; }
        [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
        public Batch Batch { get; set; }

        public int TakeCases { get; set; }
        public int TakePacks { get; set; }
        public int TakeEaches { get; set; }
        public int PlaceCases { get; set; }
        public int PlacePacks { get; set; }
        public int PlaceEaches { get; set; }

        [Ignore]
        public bool FullPallet 
        { 
            get
            {
                return TakeBin.IsFullQty(this) ?? false && AccessLevel != EAccessLevel.Ground;
            }
        }
        [Ignore]
        public EAccessLevel AccessLevel { get => TakeBin.Zone.AccessLevel; }
        [Ignore]
        public int AssignedOperator { get; set; }
        public float TimeEstimate { get; set; }

        public Move() { }

        public Move(NAVMoveLine moveLine)
        {
            ItemNumber = moveLine.ItemNumber;
            if (moveLine.ActionType == EAction.Place)
            {
                PlaceBin = moveLine.Bin;
                PlaceBinID = moveLine.BinID;
            }
            else // MUST be take.
            {
                TakeBin = moveLine.Bin;
                PlaceBinID = moveLine.BinID;
            }
        }

        public bool LineMatch(NAVMoveLine moveLine)
        {
            //TODO: This.
            throw new NotImplementedException();
        }

        public bool MergeLine(NAVMoveLine moveLine)
        {
            //TODO: This.
            throw new NotImplementedException();
        }
    }
}
