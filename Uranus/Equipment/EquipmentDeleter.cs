namespace Uranus.Equipment;

public class EquipmentDeleter
{
    public EquipmentChariot Chariot { get; set; }

    public EquipmentDeleter(ref EquipmentChariot chariot)
    {
        Chariot = chariot;
    }

}