using Microsoft.AspNetCore.Mvc;
using ParkingHUB.DTO;
using ParkingHUB.Interface;

namespace ParkingHUB.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking _parking;
        public ParkingController(IParking parking)
        {
            _parking = parking;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ParkingDTO> parkings = await _parking.GetParkings();
            return View(parkings);
        }
    }
}
