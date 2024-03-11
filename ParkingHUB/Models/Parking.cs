using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingHUB.Models
{
    public class Parking
    {
        [Key]
        public int Id { get; set; }
        public int TotalSlot {  get; set; }
        public int AvailableSlot { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }
        public byte[]? Image { get; set; }
        public ICollection<ParkingVehicle> ParkingVehicles { get; set; }


    }
}
