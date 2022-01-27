namespace Uranus.Staff
{
    public class StaffUpdater
    {
        public StaffChariot Chariot { get; set; }

        public StaffUpdater(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }

    }
}
