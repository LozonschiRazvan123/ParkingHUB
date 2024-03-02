namespace ParkingHUB.DTO
{
    public class ParkingDTO
    {
        public int Id { get; set; }
        public int TotalSlot { get; set; }
        public int AvailableSlot { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
