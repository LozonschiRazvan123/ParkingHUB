namespace ParkingHUB.ViewModel
{
    public class ParkingListViewModel
    {
        public int ParkingId {  get; set; }
        public string Location { get; set; }
        public string PlateLicense {  get; set; }
        public DateTime CheckIn {  get; set; } 
        public DateTime CheckOut {  get; set; }
        public int AvailableSlot { get; set; }
    }
}
