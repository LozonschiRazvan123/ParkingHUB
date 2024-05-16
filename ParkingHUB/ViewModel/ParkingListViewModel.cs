namespace ParkingHUB.ViewModel
{
    public class ParkingListViewModel
    {
        public int ParkingId {  get; set; }
        public int VehicleId {  get; set; }
        public string? Location { get; set; }
        public string? PlateLicense {  get; set; }
        public DateTime CheckIn {  get; set; } 
        public DateTime CheckOut {  get; set; }
        public int? AvailableSlot { get; set; }
        public double Price { get; set; }
        public double ParkingFee { get; set; }
        public int TotalSlot {  get; set; }
        public bool IsOccupied {  get; set; }
        public int NumberParking {  get; set; }
        public int CardNumber { get; set; }
        public int CVV {  get; set; }
    }
}
