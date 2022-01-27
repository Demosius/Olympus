namespace Uranus.Equipment.Model
{
    public class Stockpicker : Machine
    {
        public bool HighReach() => Type.AccessLevel == Inventory.EAccessLevel.HighReach;
    }
}
