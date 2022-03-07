namespace Uranus.Equipment;

public class EquipmentCreator
{
    public EquipmentChariot Chariot { get; set; }

    public EquipmentCreator(ref EquipmentChariot chariot)
    {
        Chariot = chariot;
    }

}