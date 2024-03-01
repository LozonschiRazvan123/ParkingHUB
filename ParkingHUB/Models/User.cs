using Microsoft.AspNetCore.Identity;

namespace ParkingHUB.Models
{
    public class User: IdentityUser
    {
        public List<Vehicle> Vehicles { get; set; }
    }
}
