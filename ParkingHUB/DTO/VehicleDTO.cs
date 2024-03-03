namespace ParkingHUB.DTO
{
    public class VehicleDTO
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public string PlateLicence { get; set; }
    }
}
