using Uranus.Equipment;
using Uranus.Inventory;
using Uranus.Staff;
using Uranus.Users;

namespace Uranus;

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

    public string SolLocation { get; }

    public Helios(string solLocation)
    {
        SolLocation = solLocation;

        inventoryChariot = new InventoryChariot(solLocation);
        InventoryCreator = new InventoryCreator(ref inventoryChariot);
        InventoryReader = new InventoryReader(ref inventoryChariot);
        InventoryUpdater = new InventoryUpdater(ref inventoryChariot);
        InventoryDeleter = new InventoryDeleter(ref inventoryChariot);

        staffChariot = new StaffChariot(solLocation);
        StaffCreator = new StaffCreator(ref staffChariot);
        StaffReader = new StaffReader(ref staffChariot);
        StaffUpdater = new StaffUpdater(ref staffChariot);
        StaffDeleter = new StaffDeleter(ref staffChariot);

        equipmentChariot = new EquipmentChariot(solLocation);
        EquipmentCreator = new EquipmentCreator(ref equipmentChariot);
        EquipmentReader = new EquipmentReader(ref equipmentChariot);
        EquipmentUpdater = new EquipmentUpdater(ref equipmentChariot);
        EquipmentDeleter = new EquipmentDeleter(ref equipmentChariot);

        userChariot = new UserChariot(solLocation);
        UserCreator = new UserCreator(ref userChariot);
        UserReader = new UserReader(ref userChariot);
        UserUpdater = new UserUpdater(ref userChariot);
        UserDeleter = new UserDeleter(ref userChariot);
    }

    public void ResetChariots(string newSol)
    {
        inventoryChariot.ResetConnection(newSol);
        staffChariot.ResetConnection(newSol);
        equipmentChariot.ResetConnection(newSol);
        userChariot.ResetConnection(newSol);
    }
}