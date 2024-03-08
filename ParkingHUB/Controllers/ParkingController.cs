using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingHUB.Data;
using ParkingHUB.DTO;
using ParkingHUB.Interface;
using ParkingHUB.Models;
using ParkingHUB.Pagination;
using ParkingHUB.Repository;
using ParkingHUB.ViewModel;

namespace ParkingHUB.Controllers
{
    public class ParkingController : Controller
    {
        private readonly IParking _parking;
        private readonly IPagination<ParkingListViewModel> _paginationRepository;
        private readonly DataContext _dataContext;
        public ParkingController(IParking parking, IPagination<ParkingListViewModel> paginationRepository, DataContext dataContext)
        {
            _parking = parking;
            _paginationRepository = paginationRepository;
            _dataContext = dataContext;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<ParkingDTO> parkings = await _parking.GetParkings();
            return View(parkings);
        }

        public async Task<IActionResult> ParkingList(int pageNumber = 1, int pageSize = 10)
        {
            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            var pageResult = await _paginationRepository.GetParkingListPagination(filter);

            return View(pageResult);
        }
        public async Task<IActionResult> ParkingListVehicle(string location, int pageNumber = 1, int pageSize = 10)
        {
            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };

            var pageResult = await _parking.GetParkingVehicleInLocation(location, filter);
            ViewBag.Location = location;
            return View(pageResult);
        }

        public async Task<IActionResult> AddParking([FromBody] ParkingListViewModel parking, int pageNumber = 1, int pageSize = 10)
        {
            /*ViewData["CheckIn"] = parking.CheckIn;
            ViewData["CheckOut"] = parking.CheckOut;
            ViewData["PlateLicense"] = parking.PlateLicense;
            ViewData["Location"] = parking.Location;*/

            var filter = new PaginationPage
            {
                pageNumber = pageNumber,
                pageSize = pageSize
            };
            if (ModelState.IsValid)
            {
                var parkingDetails = await _dataContext.Parkings
                    .FirstOrDefaultAsync(p => p.Location == parking.Location);

                if (parkingDetails != null && parkingDetails.AvailableSlot > 0)
                {
                    parkingDetails.AvailableSlot--;
                    if (parking.CheckIn < parking.CheckOut && (parking.CheckOut - parking.CheckIn).TotalHours % 1 == 0)
                    {
                        parking.ParkingFee = CalculateParkingPrice(parking.CheckIn, parking.CheckOut, parkingDetails.Price);

                        await _dataContext.SaveChangesAsync();

                        _parking.CreateParking(parking);

                        //return RedirectToAction("ParkingListVehicle", new { location = parking.Location });
                        return Json(new { showToast = true });
                    }
                    else
                    {
                        //ModelState.AddModelError("", "The difference between CheckIn and CheckOut should be an integer number of hours.");
                        //return RedirectToAction("ErrorCheckInAndCheckOut", new { location = parking.Location });
                        //_parking.CreateParking(parking);

                        //RedirectToAction("ParkingListVehicle", new { location = parking.Location });

                        //_parking.DeleteParking(parking.ParkingId);
                        //return RedirectToAction("ParkingListVehicle", new { location = parking.Location });
                        return Json(new { showToast = false });
                    }

                }
            }
                return View(parking);
        }



        private int GenerateParkingId()
        {
            return _dataContext.Parkings.Max(p => p.Id) + 1; 
        }

        public double CalculateParkingPrice(DateTime checkIn, DateTime checkOut, double pricePerHour)
        {
            TimeSpan duration = checkOut - checkIn;
            double hours = duration.TotalHours;
            double totalPrice = pricePerHour * hours;
            return totalPrice;
        }

    }
}
