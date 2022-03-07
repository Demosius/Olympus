namespace Uranus.Equipment;

public class EquipmentUpdater
{
    public EquipmentChariot Chariot { get; set; }

    public EquipmentUpdater(ref EquipmentChariot chariot)
    {
        Chariot = chariot;
    }

}