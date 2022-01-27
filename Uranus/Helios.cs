using Uranus.Inventory;
using Uranus.Equipment;
using Uranus.Staff;
using Uranus.Users;

namespace Uranus
{
    public class Helios
    {
        private readonly InventoryChariot inventoryChariot;
        public InventoryCreator InventoryCreator { get; set; }
        public InventoryReader InventoryReader { get; set; }
        public InventoryUpdater InventoryUpdater { get; set; }
        public InventoryDeleter InventoryDeleter { get; set; }

        private readonly StaffChariot staffChariot;
        public StaffCreator StaffCreator { get; set; }
        public StaffReader StaffReader { get; set; }
        public StaffUpdater StaffUpdater { get; set; }
        public StaffDeleter StaffDeleter { get; set; }

        private readonly EquipmentChariot equipmentChariot;
        public EquipmentCreator EquipmentCreator { get; set; }
        public EquipmentReader EquipmentReader { get; set; }
        public EquipmentUpdater EquipmentUpdater { get; set; }
        public EquipmentDeleter EquipmentDeleter { get; set; }

        private readonly UserChariot userChariot;
        public UserCreator UserCreator { get; set; }
        public UserReader UserReader { get; set; }
        public UserUpdater UserUpdater { get; set; }
        public UserDeleter UserDeleter { get; set; }

        public Helios(string solLocation)
        {
            inventoryChariot = new(solLocation);
            InventoryCreator = new(ref inventoryChariot);
            InventoryReader = new(ref inventoryChariot);
            InventoryUpdater = new(ref inventoryChariot);
            InventoryDeleter = new(ref inventoryChariot);

            staffChariot = new(solLocation);
            StaffCreator = new(ref staffChariot);
            StaffReader = new(ref staffChariot);
            StaffUpdater = new(ref staffChariot);
            StaffDeleter = new(ref staffChariot);

            equipmentChariot = new(solLocation);
            EquipmentCreator = new(ref equipmentChariot);
            EquipmentReader = new(ref equipmentChariot);
            EquipmentUpdater = new(ref equipmentChariot);
            EquipmentDeleter = new(ref equipmentChariot);

            userChariot = new(solLocation);
            UserCreator = new(ref userChariot);
            UserReader = new(ref userChariot);
            UserUpdater = new(ref userChariot);
            UserDeleter = new(ref userChariot);
        }

        public void ResetChariots(string newSol)
        {
            inventoryChariot.ResetConnection(newSol);
            staffChariot.ResetConnection(newSol);
            equipmentChariot.ResetConnection(newSol);
            userChariot.ResetConnection(newSol);
        }
    }
}
