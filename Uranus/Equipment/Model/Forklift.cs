namespace Uranus.Equipment.Model;

public class Forklift : Machine
{
    public bool HighReach() => Type.AccessLevel == Inventory.EAccessLevel.HighReach;
}