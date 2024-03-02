using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingHUB.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        public string PlateLicence { get; set; }
        public ICollection<ParkingVehicle> ParkingVehicles { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}
