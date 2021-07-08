using Olympus.Helios.Inventory;
using Olympus.Helios.Equipment;
using Olympus.Helios.Staff;
using Olympus.Helios.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympus.Helios
{
    public class Charioteer
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

        public Charioteer()
        {
            InventoryChariot = new InventoryChariot();
            InventoryCreator = new InventoryCreator(ref InventoryChariot);
            InventoryReader = new InventoryReader(ref InventoryChariot);
            InventoryUpdater = new InventoryUpdater(ref InventoryChariot);
            InventoryDeleter = new InventoryDeleter(ref InventoryChariot);

            StaffChariot = new StaffChariot();
            StaffCreator = new StaffCreator(ref StaffChariot);
            StaffReader = new StaffReader(ref StaffChariot);
            StaffUpdater = new StaffUpdater(ref StaffChariot);
            StaffDeleter = new StaffDeleter(ref StaffChariot);

            EquipmentChariot = new EquipmentChariot();
            EquipmentCreator = new EquipmentCreator(ref EquipmentChariot);
            EquipmentReader = new EquipmentReader(ref EquipmentChariot);
            EquipmentUpdater = new EquipmentUpdater(ref EquipmentChariot);
            EquipmentDeleter = new EquipmentDeleter(ref EquipmentChariot);

            UserChariot = new UserChariot();
            UserCreator = new UserCreator(ref UserChariot);
            UserReader = new UserReader(ref UserChariot);
            UserUpdater = new UserUpdater(ref UserChariot);
            UserDeleter = new UserDeleter(ref UserChariot);
        }

        public void ResetChariots()
        {
            InventoryChariot.ResetConnection();
            StaffChariot.ResetConnection();
            EquipmentChariot.ResetConnection();
            UserChariot.ResetConnection();
        }
    }
}
