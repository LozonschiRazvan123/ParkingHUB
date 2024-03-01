namespace ParkingHUB.Models
{
    public class ParkingVehicle
    {
        public int ParkingId { get; set; }
        public int VehicleId { get; set; }
        public Parking Parking { get; set; }
        public Vehicle Vehicle { get; set; }
    }
}
