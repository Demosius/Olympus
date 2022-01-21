using Uranus.Inventory;
using Uranus.Equipment;
using Uranus.Staff;
using Uranus.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus
{
    public class Helios
    {
        private readonly InventoryChariot InventoryChariot;
        public InventoryCreator InventoryCreator { get; set; }
        public InventoryReader InventoryReader { get; set; }
        public InventoryUpdater InventoryUpdater { get; set; }
        public InventoryDeleter InventoryDeleter { get; set; }

        private readonly StaffChariot StaffChariot;
        public StaffCreator StaffCreator { get; set; }
        public StaffReader StaffReader { get; set; }
        public StaffUpdater StaffUpdater { get; set; }
        public StaffDeleter StaffDeleter { get; set; }

        private readonly EquipmentChariot EquipmentChariot;
        public EquipmentCreator EquipmentCreator { get; set; }
        public EquipmentReader EquipmentReader { get; set; }
        public EquipmentUpdater EquipmentUpdater { get; set; }
        public EquipmentDeleter EquipmentDeleter { get; set; }

        private readonly UserChariot UserChariot;
        public UserCreator UserCreator { get; set; }
        public UserReader UserReader { get; set; }
        public UserUpdater UserUpdater { get; set; }
        public UserDeleter UserDeleter { get; set; }

        public Helios(string solLocation)
        {
            InventoryChariot = new(solLocation);
            InventoryCreator = new(ref InventoryChariot);
            InventoryReader = new(ref InventoryChariot);
            InventoryUpdater = new(ref InventoryChariot);
            InventoryDeleter = new(ref InventoryChariot);

            StaffChariot = new(solLocation);
            StaffCreator = new(ref StaffChariot);
            StaffReader = new(ref StaffChariot);
            StaffUpdater = new(ref StaffChariot);
            StaffDeleter = new(ref StaffChariot);

            EquipmentChariot = new(solLocation);
            EquipmentCreator = new(ref EquipmentChariot);
            EquipmentReader = new(ref EquipmentChariot);
            EquipmentUpdater = new(ref EquipmentChariot);
            EquipmentDeleter = new(ref EquipmentChariot);

            UserChariot = new(solLocation);
            UserCreator = new(ref UserChariot);
            UserReader = new(ref UserChariot);
            UserUpdater = new(ref UserChariot);
            UserDeleter = new(ref UserChariot);
        }

        public void ResetChariots(string newSol)
        {
            InventoryChariot.ResetConnection(newSol);
            StaffChariot.ResetConnection(newSol);
            EquipmentChariot.ResetConnection(newSol);
            UserChariot.ResetConnection(newSol);
        }
    }
}
