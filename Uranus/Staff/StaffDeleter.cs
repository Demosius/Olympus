namespace Uranus.Staff
{
    public class StaffDeleter
    {
        public StaffChariot Chariot { get; set; }

        public StaffDeleter(ref StaffChariot chariot)
        {
            Chariot = chariot;
        }
       
    }
}
